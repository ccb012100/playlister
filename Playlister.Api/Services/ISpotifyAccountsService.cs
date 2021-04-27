using System.Threading.Tasks;

namespace Playlister.Services
{
    public interface ISpotifyAccountsService
    {
        Task<object> Authorize();
    }
}
