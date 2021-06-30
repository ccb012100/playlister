using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Playlister.Models;
using Playlister.Repositories;
using Playlister.Utilities;

namespace Playlister.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistWriteRepository _playlistWriteRepository;
        private readonly IPlaylistReadRepository _playlistReadRepository;
        private readonly ILogger<PlaylistService> _logger;

        private static readonly CacheObject<Playlist> PlaylistCache = new();

        public PlaylistService(IPlaylistReadRepository playlistReadRepository, IPlaylistWriteRepository playlistWriteRepository,
            ILogger<PlaylistService> logger)
        {
            _playlistWriteRepository = playlistWriteRepository;
            _playlistReadRepository = playlistReadRepository;
            _logger = logger;

            PlaylistCache.Initialize(PopulateCache);
        }

        public Playlist? GetPlaylist(string id) => GetFromCache(id);

        public async Task UpdatePlaylist(Playlist playlist, IEnumerable<PlaylistItem> listItems, CancellationToken ct)
        {
            await _playlistWriteRepository.Upsert(playlist, listItems, ct);

            Cache(playlist);
        }

        private void Cache(Playlist playlist)
        {
            Playlist pl = PlaylistCache.Items.AddOrUpdate(playlist.Id, playlist,
                (_, b) => b == null ? throw new ArgumentNullException(nameof(b)) : playlist);

            _logger.LogTrace($"Added playlist to cache: {JsonUtility.PrettyPrint(pl)}");
        }

        private Playlist? GetFromCache(string id)
        {
            _ = PlaylistCache.Items.TryGetValue(id, out Playlist? playlist);
            _logger.LogDebug($"Result of getting playlist {id} from cache: {JsonUtility.PrettyPrint(playlist)}");
            return playlist;
        }

        private async Task PopulateCache()
        {
            IEnumerable<Playlist> playlists = await _playlistReadRepository.Get();

            _logger.LogDebug("Populating Playlist cache...");
            foreach (Playlist? playlist in playlists)
            {
                Cache(playlist);
            }

            _logger.LogTrace($"Cache populated: {JsonUtility.PrettyPrint(PlaylistCache.Items)}");
        }
    }
}
