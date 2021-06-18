using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.HttpClients;
using Playlister.Models.SpotifyApi;
using Playlister.Requests;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedType.Global

namespace Playlister.Handlers
{
    public class CurrentSpotifyUserHandler : IRequestHandler<CurrentUserRequest, PrivateUserObject>
    {
        private readonly ISpotifyApi _api;

        public CurrentSpotifyUserHandler(ISpotifyApi api)
        {
            _api = api;
        }

        public async Task<PrivateUserObject> Handle(CurrentUserRequest request, CancellationToken ct)
        {
            return await _api.GetCurrentUser(ct);
        }
    }
}
