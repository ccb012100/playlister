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
        private readonly IPlaylistWriteRepository _writeRepository;
        private readonly ISpotifyApiService _api;
        private readonly IPlaylistReadRepository _readRepository;
        private readonly ILogger<PlaylistService> _logger;

        private static readonly CacheObject<Playlist> PlaylistCache = new();

        public PlaylistService(IPlaylistReadRepository readRepository, IPlaylistWriteRepository writeRepository,
            ISpotifyApiService api, ILogger<PlaylistService> logger)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _api = api;
            _logger = logger;

            PlaylistCache.Initialize(PopulateCache);
        }

        public async Task<IEnumerable<Playlist>> GetCurrentUserPlaylistsAsync(string accessToken, CancellationToken ct)
        {
            PagingObject<SimplifiedPlaylistObject> page = await _api.GetCurrentUserPlaylistsAsync(accessToken, ct);

            List<Playlist> lists = page.Items.Select(i => i.ToPlaylist()).ToList();

            while (page.Next is not null)
            {
                page = await _api.GetCurrentUserPlaylistsAsync(accessToken, page.Next, ct);

                lists.AddRange(page.Items.Select(i => i.ToPlaylist()));
            }

            return lists;
        }

        public async Task UpdatePlaylistsAsync(string accessToken, IEnumerable<Playlist> playlists,
            CancellationToken ct)
        {
            ImmutableArray<Playlist> changedPlaylists = playlists.Where(IsChanged).ToImmutableArray();
            _logger.LogInformation($"Found {changedPlaylists.Length} changed playlists.");

            foreach (Playlist pl in changedPlaylists) await UpdatePlaylistAsync(accessToken, pl, 0, 50, ct);
        }

        #region UpdatePlaylist

        public async Task UpdatePlaylistAsync(UpdatePlaylistCommand command, CancellationToken ct)
        {
            SimplifiedPlaylistObject playlistObject =
                await _api.GetPlaylistAsync(command.AccessToken, command.PlaylistId, ct);

            Playlist playlist = playlistObject.ToPlaylist();

            if (IsChanged(playlist))
            {
                await UpdatePlaylistAsync(command.AccessToken, playlist, command.Offset, command.Limit, ct);
            }
        }

        private async Task UpdatePlaylistAsync(string accessToken, Playlist playlist, int offset, int limit,
            CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();

            // get first page of playlist items
            PagingObject<PlaylistItem> page =
                await _api.GetPlaylistTracksAsync(accessToken, playlist.Id, offset, limit, ct);

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
                $"Updated playlist {playlist.Id} (\"{playlist.Name}\"). Total time: {sw.Elapsed}");
        }

        #endregion

        private bool IsChanged(Playlist playlist)
        {
            Playlist? cachedPlaylist = GetFromCache(playlist.Id);

            // return without processing if the DB version matches the command version
            if (cachedPlaylist is not null && cachedPlaylist.SnapshotId == playlist.SnapshotId)
            {
                _logger.LogDebug($"{playlist} is unchanged since the last update.");
                return false;
            }

            _logger.LogInformation(LogInfoMessage(playlist, cachedPlaylist));

            return true;

            static string LogInfoMessage(Playlist playlist, Playlist? cachedPlaylist)
            {
                StringBuilder sb = new();
                sb.AppendLine($"{playlist} has changed since the last update:");
                sb.AppendLine($"\tSnapshotId:  {playlist.SnapshotId ?? "null"}");
                sb.AppendLine(cachedPlaylist is null
                    ? $"\tCached SnapshotId:  No cached version."
                    : $"\tCached SnapshotId:  {cachedPlaylist?.SnapshotId ?? "null"}.");

                return sb.ToString();
            }
        }

        #region cache

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
            IEnumerable<Playlist> playlists = await _readRepository.GetAllAsync();

            _logger.LogDebug("Populating Playlist cache...");
            foreach (Playlist? playlist in playlists)
            {
                Cache(playlist);
            }

            _logger.LogDebug($"Cache populated: {JsonUtility.PrettyPrint(PlaylistCache.Items)}");
        }

        #endregion
    }
}
