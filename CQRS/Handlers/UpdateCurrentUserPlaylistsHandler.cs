using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;
using Playlister.Utilities;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the current user's playlists to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdateCurrentUserPlaylistsHandler : IRequestHandler<UpdateCurrentUserPlaylistsCommand, Unit>
    {
        private readonly ISpotifyApiService _api;
        private readonly IPlaylistService _playlistService;

        public UpdateCurrentUserPlaylistsHandler(ISpotifyApiService api, IPlaylistService playlistService)
        {
            _api = api;
            _playlistService = playlistService;
        }

        public async Task<Unit> Handle(UpdateCurrentUserPlaylistsCommand command, CancellationToken ct)
        {
            await PageObjectProcessor.ProcessPages(
                async token => await _api.GetCurrentUserPlaylists(command.AccessToken, token),
                async (next, token) => await _api.GetCurrentUserPlaylists(command.AccessToken, next, token),
                async (playlistObjects, token) =>
                    await _playlistService.UpdatePlaylists(command.AccessToken, playlistObjects, token), ct);

            return new Unit();
        }
    }
}
