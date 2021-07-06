using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the Playlists in command to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistsHandler : IRequestHandler<UpdatePlaylistsCommand, int>
    {
        private readonly IPlaylistService _playlistService;

        public UpdatePlaylistsHandler(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns>Number of playlists handled</returns>
        // ReSharper disable once UseDeconstructionOnParameter
        public async Task<int> Handle(UpdatePlaylistsCommand command, CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();

            SimplifiedPlaylistObject[] playlists = command.Playlists as SimplifiedPlaylistObject[] ??
                                                   command.Playlists.ToArray();

            foreach (SimplifiedPlaylistObject playlist in playlists)
            {
                await _playlistService.UpdatePlaylist(command.AccessToken, playlist.Id, 0, 50, ct);
            }

            return playlists.Length;
        }
    }
}
