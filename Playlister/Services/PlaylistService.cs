using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Repositories;
using Playlister.Utilities;

namespace Playlister.Services
{
    public class PlaylistService : IPlaylistService
    {
        private static readonly CacheObject<Playlist> s_playlistCache = new();
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

            s_playlistCache.Initialize(PopulateCache);
        }

        public async Task<IEnumerable<Playlist>> GetCurrentUserPlaylistsAsync(
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

            return lists;
        }

        public async Task UpdatePlaylistsAsync(
            string accessToken,
            IEnumerable<Playlist> playlists,
            CancellationToken ct
        )
        {
            ImmutableArray<Playlist> changedPlaylists = playlists
                .Where(IsChanged)
                .ToImmutableArray();

            _logger.LogInformation("Found {Length} changed playlists", changedPlaylists.Length);

            foreach (Playlist pl in changedPlaylists.AsParallel())
            {
                await UpdatePlaylistAsync(accessToken, pl, 0, 50, ct);
            }
        }

        private bool IsChanged(Playlist playlist)
        {
            Playlist? cachedPlaylist = GetFromCache(playlist.Id);

            // return without processing if the DB version matches the command version
            if (cachedPlaylist is not null && cachedPlaylist.SnapshotId == playlist.SnapshotId
                /*  If the counts are different then they've gotten out of sync,
                 *  usually because tracks have been deleted from the playlist */
                && cachedPlaylist.Count == playlist.Count)
            {
                _logger.LogDebug("{Playlist} is unchanged since the last update", playlist);
                return false;
            }

            _logger.LogInformation("{}", LogInfoMessage(playlist, cachedPlaylist));

            return true;

            static string LogInfoMessage(Playlist playlist, Playlist? cachedPlaylist)
            {
                StringBuilder sb = new();

                sb.AppendLine($"{playlist} has changed since the last update:");
                sb.AppendLine($"\tSnapshotId:         {playlist.SnapshotId ?? "null"}");
                sb.AppendLine(
                    cachedPlaylist is null
                        ? "\tCached SnapshotId:  [No cached version]"
                        : $"\tCached SnapshotId:  {cachedPlaylist.SnapshotId ?? "null"}"
                );

                return sb.ToString();
            }
        }

        #region UpdatePlaylist

        public async Task UpdatePlaylistAsync(UpdatePlaylistCommand command, CancellationToken ct)
        {
            SimplifiedPlaylistObject playlistObject = await _api.GetPlaylistAsync(
                command.AccessToken,
                command.PlaylistId,
                ct
            );

            Playlist playlist = playlistObject.ToPlaylist();

            if (IsChanged(playlist))
                await UpdatePlaylistAsync(
                    command.AccessToken,
                    playlist,
                    command.Offset,
                    command.Limit,
                    ct
                );
        }

        private async Task UpdatePlaylistAsync(
            string accessToken,
            Playlist playlist,
            int offset,
            int limit,
            CancellationToken ct
        )
        {
            _logger.LogInformation(
                "Updating playlist {PlaylistId} (\"{PlaylistName}\")...",
                playlist.Id,
                playlist.Name
            );
            var sw = new Stopwatch();
            sw.Start();

            // get first page of playlist items
            PagingObject<PlaylistItem> page = await _api.GetPlaylistTracksAsync(
                accessToken,
                playlist.Id,
                offset,
                limit,
                ct
            );

            /* NOTE: this takes 10s of seconds to udpate the largest playlists (once the track count starts getting into
             * the thousands; I would like to update this to only grab changes made after the last sync, but the
             * Spotify API's GetPlaylistItems endpoint does not allow filtering or ordering */

            // We want to get all items so that they can be inserted into the repository in a single Transaction
            List<PlaylistItem> allItems = page.Items.ToList();

            while (page.Next is not null)
            {
                page = await _api.GetPlaylistTracksAsync(accessToken, page.Next, ct);
                allItems.AddRange(page.Items);
            }

            await _writeRepository.UpsertAsync(playlist, allItems, ct);

            // only cache after data has been written to database
            Cache(playlist);

            sw.Stop();
            _logger.LogInformation(
                "\n=> Updated playlist {PlaylistId} (\"{PlaylistName}\").\nTotal time: {Elapsed}ms\n",
                playlist.Id,
                playlist.Name,
                sw.Elapsed.TotalMilliseconds
            );
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

            _logger.LogDebug("Added playlist to cache: {Playlist}", JsonUtility.PrettyPrint(pl));
        }

        private Playlist? GetFromCache(string id)
        {
            _ = s_playlistCache.Items.TryGetValue(id, out Playlist? playlist);
            _logger.LogDebug(
                "Result of getting playlist {Id} from cache: {playlist}",
                id,
                JsonUtility.PrettyPrint(playlist)
            );
            return playlist;
        }

        private async Task PopulateCache()
        {
            IEnumerable<Playlist> playlists = await _readRepository.GetAllAsync();

            _logger.LogDebug("Populating Playlist cache...");
            foreach (Playlist? playlist in playlists)
                Cache(playlist);

            _logger.LogDebug(
                "Cache populated: {CacheItems}",
                JsonUtility.PrettyPrint(s_playlistCache.Items)
            );
        }

        #endregion
    }
}
