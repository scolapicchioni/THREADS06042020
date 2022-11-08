using System.Collections.Concurrent;

namespace SlowLibrary; 
public static class SlowLibraryCacheService {
    private static ConcurrentDictionary<int, int> cache = new ConcurrentDictionary<int, int>();

    public static Task<int> GetSlowSquare(int number) {
        if (cache.ContainsKey(number)) {
            return Task.FromResult(cache[number]);
        } else {
            SlowClass slowClass = new SlowClass();
            Task<int> t = Task.Run(()=>slowClass.SlowMethod04(number));
            t.ContinueWith(t=>cache.TryAdd(number, t.Result));
            return t;
        }
    }
}
