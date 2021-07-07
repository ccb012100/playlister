using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the current user's playlists to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdateCurrentUserPlaylistsHandler : IRequestHandler<UpdateCurrentUserPlaylistsCommand, int>
    {
        private readonly IPlaylistService _playlistService;

        public UpdateCurrentUserPlaylistsHandler(IPlaylistService playlistService) =>
            _playlistService = playlistService;

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns>Number of playlists handled.</returns>
        public async Task<int> Handle(UpdateCurrentUserPlaylistsCommand command, CancellationToken ct)
        {
            ImmutableArray<Playlist> playlists = (await
                _playlistService.GetCurrentUserPlaylistsAsync(command.AccessToken, ct)!).ToImmutableArray();

            await _playlistService.UpdatePlaylistsAsync(command.AccessToken, playlists, ct);

            return playlists.Length;
        }
    }
}
