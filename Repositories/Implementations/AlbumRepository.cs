using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories.Implementations
{
    public class AlbumRepository : IAlbumRepository
    {
        public Task Upsert(IEnumerable<Album> albums, CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }
    }
}