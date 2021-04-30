using System.Threading.Tasks;

namespace Playlister.Api.Services
{
    public interface ISpotifyAccountsService
    {
        Task<object> Authorize();
    }
}
