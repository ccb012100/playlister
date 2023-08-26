using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Playlister.Models
{
    public class CacheObject<T>
    {
        public bool Initialized { get; set; }
        public ConcurrentDictionary<string, T> Items { get; } = new();

        public async void Initialize(Func<Task> initializationFn)
        {
            if (Initialized) return;

            await initializationFn();

            Initialized = true;
        }
    }
}
