using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories.Implementations
{
    public class ArtistRepository : IArtistRepository
    {
        public Task Upset(IEnumerable<Artist> artists, CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }
    }
}