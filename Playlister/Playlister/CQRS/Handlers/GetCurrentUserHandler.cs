using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;
using Playlister.RefitClients;

namespace Playlister.CQRS.Handlers
{
    public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserCommand, PrivateUserObject>
    {
        private readonly ISpotifyApi _api;

        public GetCurrentUserHandler(ISpotifyApi api)
        {
            _api = api;
        }

        public async Task<PrivateUserObject> Handle(GetCurrentUserCommand command, CancellationToken ct) =>
            await _api.GetCurrentUserAsync(command.AccessToken, ct);
    }
}
