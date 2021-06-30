using System.Collections.Generic;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories
{
    public interface IPlaylistReadRepository
    {
        Task<Playlist?> Get(string id);
        Task<IEnumerable<Playlist>> Get();
    }
}
