using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories
{
    public interface IAlbumRepository
    {
        Task Upsert(IEnumerable<Album> albums, CancellationToken ct);
    }
}