using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisDemo.Extensions
{
    // Is better to implement IDistributedCache
    public static class DistributedCached
    {
        /// <summary>
        /// Set Record to cache async
        /// </summary>
        /// <param name="cach"></param>
        /// <param name="recordId"></param>
        /// <param name="data"></param>
        /// <param name="absoluteExpireTime"></param>
        /// <param name="unusedExpireTime"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task SetRecordAsync<T>(this IDistributedCache cach,
        string recordId,
        T data,
        TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null)
        {

            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60); // 1Min
            options.SlidingExpiration = unusedExpireTime;

            var jsonData = JsonSerializer.Serialize(data);

            await cach.SetStringAsync(recordId, jsonData, options);
        }

        /// <summary>
        /// Get Record By RecordId Async
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="recordsId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordsId)
        {
            var jsonData = await cache.GetStringAsync(recordsId);

            if (jsonData is null) return default(T);

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}