using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Commands;

namespace Playlister.CQRS.Handlers
{
    public class SpotifyAuthUrlHandler : IRequestHandler<GetAuthUrlCommand, Uri>
    {
        private const string Scope = "user-read-private";
        private readonly SpotifyOptions _options;

        public SpotifyAuthUrlHandler(IOptions<SpotifyOptions> options)
        {
            _options = options.Value;

            if (string.IsNullOrWhiteSpace(_options.ClientId))
            {
                throw new Exception(
                    "Client Id is not present. Make sure that the project secrets have been set"
                );
            }
        }

        public Task<Uri> Handle(GetAuthUrlCommand command, CancellationToken ct)
        {
            /*
             * https://accounts.spotify.com/authorize?
             * client_id=5fe01282e44241328a84e7c5cc169165
             * &response_type=code
             * &redirect_uri=https%3A%2F%2Fexample.com%2Fcallback
             * &scope=user-read-private%20user-read-email
             * &state=34fFs29kd09
             */
            StringBuilder builder = new StringBuilder(
                _options.AccountsApiBaseAddress.OriginalString
            )
                .Append("/authorize?")
                .Append("response_type=code")
                .Append($"&client_id={_options.ClientId}")
                .Append($"&redirect_uri={_options.CallbackUrl}")
                .Append($"&scope={Scope}")
                .Append($"&state={Guid.NewGuid()}"); // TODO: cache `state` so that it can be validated on access token command

            return Task.FromResult(new Uri(builder.ToString()));
        }
    }
}
