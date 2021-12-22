using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Fly.Common.Extensions
{
    public static class DistributedCaching
    {
        public async static Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, CancellationToken token = default(CancellationToken))
        {
            var timeOut = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            distributedCache.SetString(key, JsonSerializer.Serialize(value), timeOut);
        }

        public async static Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var value = distributedCache.GetString(key);

            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }
    }
}
