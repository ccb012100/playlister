using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Commands;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Repositories;
using Playlister.Utilities;

namespace Playlister.Services
{
    public class PlaylistService : IPlaylistService
    {
        private static readonly CacheObject<Playlist> s_playlistCache = new();
        private static readonly CacheObject<string> s_missingTracksCache = new();
        private static readonly CacheObject<string> s_updatedPlaylistsCache = new();
        private readonly ISpotifyApiService _api;
        private readonly ILogger<PlaylistService> _logger;
        private readonly IPlaylistReadRepository _readRepository;
        private readonly IPlaylistWriteRepository _writeRepository;

        public PlaylistService(
            IPlaylistReadRepository readRepository,
            IPlaylistWriteRepository writeRepository,
            ISpotifyApiService api,
            ILogger<PlaylistService> logger
        )
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _api = api;
            _logger = logger;

            _ = s_playlistCache.Initialize(PopulateCaches);
        }

        /// <summary>
        ///     Update the data for the specified Playlist
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="playlist"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="ct"></param>
        /// <param name="forceSync">If <see langword="true" />, sync the playlist regardless of whether it's changed since the last sync</param>
        /// <returns></returns>
        private async Task UpdatePlaylistAsync(
            string accessToken,
            Playlist playlist,
            int offset,
            int limit,
            CancellationToken ct,
            bool forceSync = false
        )
        {
            _logger.LogDebug(
                "{Playlist} Updating playlist...",
                playlist.LoggingTag
            );

            if (!forceSync && IsCurrent(playlist) && HasAllTracks(playlist))
            {
                _logger.LogDebug("{PlaylistTag} playlist is up-to-date. Skipping sync", playlist.LoggingTag);

                return;
            }

            Stopwatch sw = new();
            sw.Start();

            // get first page of playlist items
            PagingObject<PlaylistItem> page = await _api.GetPlaylistTracksAsync(
                accessToken,
                playlist.Id,
                offset,
                limit,
                ct
            );

            /*
             * NOTE: this takes 10s of seconds to udpate the largest playlists (once the track count starts getting into
             * the thousands; I would like to update this to only grab changes made after the last sync, but the
             * Spotify API's GetPlaylistItems endpoint does not allow filtering or ordering
             */

            /*
             * PERF: Grab the first page and then calculate the number of remaining pages based on (total/limit).
             *       Then grab those pages in parallel and combine into a single collection.
             */

            // We want to get all the items for the playlist so that they can be inserted into the repository in a single Transaction
            List<PlaylistItem> allItems = page.Items.ToList();

            while (page.Next is not null)
            {
                page = await _api.GetPlaylistTracksAsync(accessToken, page.Next, ct);
                allItems.AddRange(page.Items);
            }

            if (allItems.Count != playlist.Count)
            {
                _logger.LogWarning(
                    "{PlaylistTag} The number of tracks returned from the API does not match Playlist.Count. Expected: {PlaylistCount}. Actual: {PlaylistTrackCount}",
                    playlist.LoggingTag, playlist.Count, allItems.Count);
            }

            ImmutableArray<PlaylistItem> uniqueTracks = allItems.DistinctBy(x =>
                    new DistinctPlaylistTracks.DistinctPlaylistTrack(playlist.Id, x.Track.Id, x.AddedAt))
                .ToImmutableArray();

            playlist = playlist with { CountUnique = uniqueTracks.Length };

            _logger.LogInformation("{PlaylistTag} playlist contains {CountUnique} unique tracks (out of {Count})", playlist.LoggingTag,
                uniqueTracks.Length, playlist.Count);

            await _writeRepository.UpsertAsync(playlist, uniqueTracks, ct);

            // only cache after data has been written to database
            Cache(playlist);
            CacheUpdatedPlaylist(playlist);
            DecacheMissingTracks(playlist);

            sw.Stop();

            _logger.LogInformation(
                "{PlaylistTag} Updated playlist. Total time: {Elapsed}\n",
                playlist.LoggingTag,
                sw.Elapsed.ToLogString()
            );
        }

        /// <summary>
        ///     Indicates whether the database needs to be synced with Spotify for the specified Playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        private bool IsCurrent(Playlist playlist)
        {
            Playlist? cachedPlaylist = GetFromCache(playlist.Id);

            if (cachedPlaylist is null) // If the playlist isn't in the cache, then we haven't synced it before
            {
                _logger.LogDebug("{PlaylistTag} was not found in the cache:\n\tSnapshotId:\t{SnapshotId}", playlist.LoggingTag, playlist.SnapshotId);

                return false;
            }

            if (cachedPlaylist.SnapshotId != playlist.SnapshotId)
            {
                _logger.LogInformation(
                    "{PlaylistTag} has changed since the last update:\n\tSnapshotId:         {SnapshotId}\n\tCached SnapshotId:  {CachedSnapshotId}",
                    playlist.LoggingTag,
                    playlist.SnapshotId,
                    cachedPlaylist.SnapshotId
                );

                return false;
            }

            // if the SnapshotIds match, it hasn't changed since the last sync
            _logger.LogDebug("{PlaylistTag} is unchanged since the last sync", playlist.LoggingTag);

            return true;
        }

        /// <summary>
        ///     Indicates whether the database contains all the PlaylistTracks for the specified Playlist.
        ///     If they do not match, it's most likely because there have been tracks deleted from the Playlist in Spotify.
        ///     Note that we're using a simple heuristic based on count of PlaylistTracks in the database; it's possible for the count to be the same but the
        ///     tracks aren't the correct ones, but that would be a bug in the syncing logic that this does not (and should not) account for.
        /// </summary>
        /// <remarks>
        ///     Right now, this can't be hooked into the normal Playlist update logic because we don't store duplicates in
        ///     Playlists and Spotify's definition of a "duplicate" is not entirely clear (singles seem to be treated as a
        ///     duplicate of the album track in at least some cases).
        /// </remarks>
        /// <param name="playlist"></param>
        /// <returns>
        ///     <see langword="false" /> if the number of <see cref="PlaylistTrack" />s for <paramref name="playlist" /> in the database is less than its
        ///     <see cref="Playlist.Count" /> property; Otherwise, <see langword="true" />
        /// </returns>
        private static bool HasAllTracks(Playlist playlist)
        {
            bool found = s_missingTracksCache.Items.ContainsKey(playlist.Id);

            // if the playlist isn't in the cache, it has all its tracks
            return !found;
        }

        #region public

        public async Task<ImmutableArray<Playlist>> GetCurrentUserPlaylistsAsync(
            string accessToken,
            CancellationToken ct
        )
        {
            PagingObject<SimplifiedPlaylistObject> page = await _api.GetCurrentUserPlaylistsAsync(
                accessToken,
                ct
            );

            List<Playlist> lists = page.Items.Select(i => i.ToPlaylist()).ToList();

            while (page.Next is not null)
            {
                page = await _api.GetCurrentUserPlaylistsAsync(accessToken, page.Next, ct);

                lists.AddRange(page.Items.Select(i => i.ToPlaylist()));
            }

            return lists.ToImmutableArray();
        }

        public async Task UpdatePlaylistsAsync(
            string accessToken,
            IEnumerable<Playlist> playlists,
            CancellationToken ct
        )
        {
            Stopwatch sw = Stopwatch.StartNew();

            foreach (Playlist playlist in playlists.AsParallel())
            {
                await UpdatePlaylistAsync(accessToken, playlist, 0, 50, ct);
            }

            sw.Stop();

            if (s_updatedPlaylistsCache.Items.Any())
            {
                _logger.LogInformation(
                    "It took {Elapsed} seconds to update the {ChangedPlaylistCount} changed playlists",
                    sw.Elapsed.ToLogString(),
                    s_updatedPlaylistsCache.Items.Count
                );
            }
            else
            {
                _logger.LogInformation(
                    "There were no changed playlists found. Time elapsed: {Elapsed}",
                    sw.Elapsed.ToLogString()
                );
            }
        }

        public async Task SyncPlaylistAsync(
            string accessToken,
            string playlistId,
            CancellationToken ct
        )
        {
            SimplifiedPlaylistObject playlistObject = await _api.GetPlaylistAsync(accessToken, playlistId, ct);

            Playlist playlist = playlistObject.ToPlaylist();

            await UpdatePlaylistAsync(accessToken, playlist, 0, 50, ct);
        }

        public async Task UpdatePlaylistAsync(UpdatePlaylistCommand command, CancellationToken ct)
        {
            SimplifiedPlaylistObject playlistObject = await _api.GetPlaylistAsync(
                command.AccessToken,
                command.PlaylistId,
                ct
            );

            Playlist playlist = playlistObject.ToPlaylist();

            await UpdatePlaylistAsync(
                command.AccessToken,
                playlist,
                command.Offset,
                command.Limit,
                ct
            );
        }

        public async Task DeleteOrphanedPlaylistTracksAsync(CancellationToken ct)
        {
            _logger.LogDebug("Deleting orphaned PlaylistTracks...");

            await _writeRepository.DeleteOrphanedPlaylistTracksAsync(ct);
        }

        #endregion

        #region cache

        private void Cache(Playlist playlist)
        {
            Playlist pl = s_playlistCache.Items.AddOrUpdate(
                playlist.Id,
                playlist,
                (_, b) => b == null ? throw new ArgumentNullException(nameof(b)) : playlist
            );

            _logger.LogDebug("{PlaylistTag} Added playlist to the cache: {Playlist}", playlist.LoggingTag, JsonUtility.PrettyPrint(pl));
        }

        private Playlist? GetFromCache(string id)
        {
            bool found = s_playlistCache.Items.TryGetValue(id, out Playlist? playlist);

            if (found)
            {
                _logger.LogDebug("Found playlist {PlaylistId} in the cache: {Playlist}", id, JsonUtility.PrettyPrint(playlist));
            }
            else
            {
                _logger.LogDebug("Playlist {PlaylistId} was not present in the cache", id);
            }

            return playlist;
        }

        private void CacheMissingTracks((string, int) playlistWithCount)
        {
            (string playlistId, int count) = playlistWithCount;

            string cacheItem = s_missingTracksCache.Items.AddOrUpdate(
                playlistId,
                count.ToString(),
                (_, b) => b == null ? throw new ArgumentNullException(nameof(b)) : count.ToString()
            );

            _logger.LogDebug("Added playlist {PlaylistId} to the MissingTracks cache; Count = {Count}", playlistId, cacheItem);
        }

        private void DecacheMissingTracks(Playlist playlist)
        {
            if (s_missingTracksCache.Items.Remove(playlist.Id, out string? _))
            {
                _logger.LogDebug("{PlaylistTag} Removed playlist from the MissingTracks cache", playlist.LoggingTag);
            }
            else
            {
                _logger.LogDebug("{PlaylistTag} Playlist was not present in the MissingTracks cache", playlist.LoggingTag);
            }
        }

        private void CacheUpdatedPlaylist(Playlist playlist)
        {
            bool added = s_updatedPlaylistsCache.Items.TryAdd(playlist.Id, playlist.Id);

            if (added)
            {
                _logger.LogDebug("{PlaylistTag} Added playlist to the UpdatedPlaylists cache", playlist.LoggingTag);
            }
            else
            {
                _logger.LogWarning("{PlaylistTag} Playlist was already in the UpdatedPlaylists cache", playlist.LoggingTag);
            }
        }

        /// <summary>
        ///     Populate <see cref="s_playlistCache" /> and <see cref="s_missingTracksCache" />
        /// </summary>
        /// <returns></returns>
        private async Task PopulateCaches()
        {
            List<Task> tasks = new() { PopulatePlaylistCache(), PopulateMissingTracksCache() };

            await Task.WhenAll(tasks);

            async Task PopulatePlaylistCache()
            {
                _logger.LogDebug("Populating Playlist cache...");

                IEnumerable<Playlist> playlists = await _readRepository.GetAllAsync();

                playlists.AsParallel().ForAll(Cache);

                _logger.LogDebug(
                    "Playlist cache populated: {CacheItems}",
                    JsonUtility.PrettyPrint(s_playlistCache.Items)
                );
            }

            async Task PopulateMissingTracksCache()
            {
                _logger.LogDebug("Populating MissingTracks cache...");

                IEnumerable<(string, int)> missingTracks = await _readRepository.GetPlaylistsWithMissingTracksAsync();

                missingTracks.AsParallel().ForAll(CacheMissingTracks);

                _logger.LogDebug(
                    "MissingTracks cache populated: {CacheItems}",
                    JsonUtility.PrettyPrint(s_missingTracksCache.Items)
                );
            }
        }

        #endregion
    }
}
