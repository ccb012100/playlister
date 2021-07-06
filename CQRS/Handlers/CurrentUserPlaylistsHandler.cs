using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class CurrentUserPlaylistsHandler : IRequestHandler<GetCurrentUserPlaylistsCommand,
        IEnumerable<Playlist>>
    {
        private readonly ISpotifyApiService _service;

        public CurrentUserPlaylistsHandler(ISpotifyApiService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<Playlist>> Handle(GetCurrentUserPlaylistsCommand command,
            CancellationToken ct)
        {
            PagingObject<SimplifiedPlaylistObject> page =
                await _service.GetCurrentUserPlaylists(command.AccessToken, ct);

            List<Playlist> lists = page.Items.Select(i => i.ToPlaylist()).ToList();

            while (page.Next is not null)
            {
                page = await _service.GetCurrentUserPlaylists(command.AccessToken, page.Next, ct);

                lists.AddRange(page.Items.Select(i => i.ToPlaylist()));
            }

            return lists;
        }
    }
}
