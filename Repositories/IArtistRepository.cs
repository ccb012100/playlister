using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories
{
    public interface IArtistRepository
    {
        Task Upset(IEnumerable<Artist> artists, CancellationToken ct);
    }
}