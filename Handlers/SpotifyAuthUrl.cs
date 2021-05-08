using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;

namespace Playlister.Handlers
{
    public class SpotifyAuthUrl : IRequestHandler<AuthUrlRequest, Uri>
    {
        private readonly SpotifyOptions _options;
        private const string Scope = "user-read-private";

        public SpotifyAuthUrl(IOptions<SpotifyOptions> options)
        {
            _options = options.Value;
        }

        public Task<Uri> Handle(AuthUrlRequest request, CancellationToken cancellationToken)
        {
            // https://accounts.spotify.com/authorize?
            // client_id=5fe01282e44241328a84e7c5cc169165
            // &response_type=code
            // &redirect_uri=https%3A%2F%2Fexample.com%2Fcallback
            // &scope=user-read-private%20user-read-email
            // &state=34fFs29kd09
            var builder = new StringBuilder(_options.AccountsApiBaseAddress.OriginalString);
            builder.Append("/authorize?")
                .Append($"client_id={_options.ClientId}")
                .Append($"&redirect_uri={_options.CallbackUrl}")
                .Append($"&scope={Scope}")
                .Append($"&state={Guid.NewGuid()}");
            throw new NotImplementedException();
        }
    }
}