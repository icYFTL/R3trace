using System.Runtime.Caching;

namespace R3TraceShared.extensions;

public static class CacheExtensions
{
    public static TResult Cache<TResult, TExec>(this TExec anyObject, Func<TExec, TResult> item, string key, TimeSpan timeSpan = default, bool isAbsolute = true)
    {
        var cache = MemoryCache.Default;
        if (cache.Contains(key))
            return (TResult)cache.Get(key);

        if (timeSpan == default)
            timeSpan = TimeSpan.FromMinutes(10);

        var cacheItemPolicy = isAbsolute ? new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(timeSpan) } : new CacheItemPolicy { SlidingExpiration = timeSpan };

        var result = item.Invoke(anyObject);
        cache.Add(key, result, cacheItemPolicy);
        return result;
    }
}
