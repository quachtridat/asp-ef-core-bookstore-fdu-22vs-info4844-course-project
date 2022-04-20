using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CourseProject.Services {
    public class SimpleMemoryCache {
        public struct Options {
            public long Size { get; set; } = 1;
            public TimeSpan AbsoluteExpiration { get; set; } = TimeSpan.FromSeconds(10);
            public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromSeconds(10);
            public TimeSpan AutoExpiration { get; set; } = TimeSpan.FromSeconds(10);

            public static Options Default { get; } = new Options();
        }

        public IMemoryCache Cache { get; protected set; }

        public SimpleMemoryCache() {
            Cache = new MemoryCache(
                new MemoryCacheOptions {
                    SizeLimit = 1024
                }
            );
        }
        public object? Get(object key) {
            if (Cache.TryGetValue(key, out object value)) return value;
            return null;
        }
        public T? Get<T>(object key) {
            if (Cache.TryGetValue(key, out T value)) return value;
            return default;
        }
        public void Set<TValue>(object key, TValue value, Options? options = null, bool clear = false) {
            if (clear) Cache.Remove(key);

            if (options is null || !options.HasValue) options = Options.Default;

            var cts = new CancellationTokenSource(options.Value.AutoExpiration);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(options.Value.Size)
                .SetAbsoluteExpiration(options.Value.AbsoluteExpiration)
                .SetSlidingExpiration(options.Value.SlidingExpiration)
                .AddExpirationToken(new CancellationChangeToken(cts.Token));

            Cache.Set(key, value, cacheEntryOptions);
        }
        public TValue GetSet<TValue>(object key, Func<TValue> valueFunc, Options? options = null, bool forceClear = false) {
            if (options is null || !options.HasValue) options = Options.Default;

            if (!forceClear && Cache.TryGetValue(key, out TValue cachedValue))
                return cachedValue;
             
            TValue value = valueFunc();

            Set(key, value, options, forceClear);

            return value;
        }
        public async Task<TValue> GetSetAsync<TValue>(object key, Func<Task<TValue>> valueFuncAsync, Options? options = null, bool forceClear = false) {
            if (options is null || !options.HasValue) options = Options.Default;

            if (!forceClear && Cache.TryGetValue(key, out TValue cachedValue))
                return cachedValue;

            TValue value = await valueFuncAsync();

            Set(key, value, options, forceClear);

            return value;
        }
    }
}
