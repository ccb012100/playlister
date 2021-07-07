using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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
        private readonly IPlaylistWriteRepository _playlistWriteRepository;
        private readonly ISpotifyApiService _api;
        private readonly IPlaylistReadRepository _playlistReadRepository;
        private readonly ILogger<PlaylistService> _logger;

        private static readonly CacheObject<Playlist> PlaylistCache = new();

        public PlaylistService(IPlaylistReadRepository playlistReadRepository,
            IPlaylistWriteRepository playlistWriteRepository, ISpotifyApiService api, ILogger<PlaylistService> logger)
        {
            _playlistWriteRepository = playlistWriteRepository;
            _api = api;
            _playlistReadRepository = playlistReadRepository;
            _logger = logger;

            PlaylistCache.Initialize(PopulateCache);
        }

        public async Task UpdatePlaylistsAsync(string accessToken, IEnumerable<SimplifiedPlaylistObject> playlists,
            CancellationToken ct)
        {
            ImmutableArray<SimplifiedPlaylistObject> playlistArray = playlists.ToImmutableArray();

            foreach (SimplifiedPlaylistObject playlist in playlistArray)
            {
                await UpdatePlaylistAsync(accessToken, playlist.Id, 0, 50, ct);
            }
        }

        public Task UpdatePlaylistAsync(UpdatePlaylistCommand updateCommand, CancellationToken ct) =>
            UpdatePlaylistAsync(updateCommand.AccessToken, updateCommand.PlaylistId, updateCommand.Offset,
                updateCommand.Limit, ct);

        public async Task UpdatePlaylistAsync(string accessToken, string playlistId, int offset, int limit,
            CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();

            Playlist? playlist = GetFromCache(playlistId);
            SimplifiedPlaylistObject playlistObject = await _api.GetPlaylistAsync(accessToken, playlistId, ct);

            // return without processing if the DB version matches the command version
            if (playlist is not null && playlist.SnapshotId == playlistObject.SnapshotId)
            {
                _logger.LogInformation(
                    $"Playlist `{playlistId}` (playlist name `{playlist.Name}`) hasn't changed since the last update. Not proceeding further.");
                sw.Stop();

                return;
            }

            // get first page of playlist items
            PagingObject<PlaylistItem> page = await _api.GetPlaylistTracksAsync(accessToken, playlistId, offset, limit, ct);

            // We want to get all items so that they can be inserted into the repository in a single Transaction
            List<PlaylistItem> allItems = page.Items.ToList();

            while (page.Next is not null)
            {
                page = await _api.GetPlaylistTracksAsync(accessToken, page.Next, ct);
                allItems.AddRange(page.Items);
            }

            playlist = playlistObject.ToPlaylist();
            await _playlistWriteRepository.UpsertAsync(playlist, allItems, ct);
            Cache(playlist);
            sw.Stop();
            _logger.LogInformation($"Updated playlist {playlist.Id} \"{playlist.Name}\n. Total time: {sw.Elapsed}");
        }

        private void Cache(Playlist playlist)
        {
            Playlist pl = PlaylistCache.Items.AddOrUpdate(playlist.Id, playlist,
                (_, b) => b == null ? throw new ArgumentNullException(nameof(b)) : playlist);

            _logger.LogDebug($"Added playlist to cache: {JsonUtility.PrettyPrint(pl)}");
        }

        private Playlist? GetFromCache(string id)
        {
            _ = PlaylistCache.Items.TryGetValue(id, out Playlist? playlist);
            _logger.LogDebug($"Result of getting playlist {id} from cache: {JsonUtility.PrettyPrint(playlist)}");
            return playlist;
        }

        private async Task PopulateCache()
        {
            IEnumerable<Playlist> playlists = await _playlistReadRepository.GetAllAsync();

            _logger.LogDebug("Populating Playlist cache...");
            foreach (Playlist? playlist in playlists)
            {
                Cache(playlist);
            }

            _logger.LogDebug($"Cache populated: {JsonUtility.PrettyPrint(PlaylistCache.Items)}");
        }
    }
}
