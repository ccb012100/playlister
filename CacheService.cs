using Microsoft.Extensions.Caching.Memory;

namespace Playlister
{
    public class CacheService
    {
        // TODO: add static cache
        // TODO: cache DB tables in memory
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
    }
}
