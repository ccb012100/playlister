using System.Collections.Generic;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories
{
    public interface IPlaylistReadRepository
    {
        /// <summary>
        /// Get all playlists from database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Playlist>> GetAllAsync();
    }
}
