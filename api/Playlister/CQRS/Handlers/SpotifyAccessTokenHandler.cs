using MediatR;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.RefitClients;

namespace Playlister.CQRS.Handlers;

public class SpotifyAccessTokenHandler : IRequestHandler<GetAccessTokenCommand, UserAccessToken>
{
    private readonly ISpotifyAccountsApi _api;
    private readonly SpotifyOptions _options;

    public SpotifyAccessTokenHandler(ISpotifyAccountsApi api, IOptions<SpotifyOptions> options)
    {
        _api = api;
        _options = options.Value;
    }

    public async Task<UserAccessToken> Handle(GetAccessTokenCommand command, CancellationToken ct)
    {
        // TODO: validate that the `state` value matches the original value sent to user
        // TODO: Generate a client token to return so that the Spotify Access Token is never exposed outside the API
        SpotifyAccessToken token = await _api.RequestAccessTokenAsync(
            new GetAccessTokenCommand.BodyParams
            {
                Code = command.Code,
                RedirectUri = _options.CallbackUrl.ToString(),
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            }, ct);

        return token.ToUserAccessToken();
    }
}
