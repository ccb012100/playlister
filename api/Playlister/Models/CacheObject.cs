using System.Collections.Concurrent;

namespace Playlister.Models;

public class CacheObject<T>
{
    private bool Initialized { get; set; }
    public ConcurrentDictionary<string, T> Items { get; } = new();

    public async Task Initialize(Func<Task> initializationFn)
    {
        if (Initialized)
        {
            return;
        }

        await initializationFn();

        Initialized = true;
    }
}
