using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Finbourne.Cache.Component.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Finbourne.Cache.Component
{
    public class FinbourneCacheService : IFinbourneCacheService
    {
        private readonly MemoryCache _memoryCache;
        private readonly long _sizeLimit;
        private readonly double _expirationInSeconds;

        public FinbourneCacheService(IFinbourneMemoryCache memoryCache)
        {
            _memoryCache = memoryCache.Get();
            _sizeLimit = memoryCache.SizeLimit;
            _expirationInSeconds = double.Parse(ConfigurationManager.AppSettings["Finbourne.MemoryCache.ExpirationInSeconds"]);
        }

        private SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);


        public T Get<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T type))
                return type;

            throw new Exception($"{key} does not exist in cache");

        }

        public async Task Set<T>(string key, T type)
        {
            // check to see if the item is already in cache
            if (_memoryCache.TryGetValue(key, out T cachedType))
                return;

            await _connectionLock.WaitAsync();

            try
            {
                // check to see if the item is already in cache to handle race condition
                if (_memoryCache.TryGetValue(key, out cachedType))
                    return;

                // item still doesnt exits in cache so lets persist the data to cache
                // we use sliding so highly used cached items will continue to be renewed.
                // we have to set the size of each item we add
                // when we forced an eviction we want the details of it so we can forward the information to consumer
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                                .SetSlidingExpiration(TimeSpan.FromSeconds(_expirationInSeconds))
                                                .SetSize(1)
                                                .SetPriority(CacheItemPriority.Normal)
                                                .RegisterPostEvictionCallback(PostEvictionCallback, _memoryCache);


                var stats = _memoryCache.GetCurrentStatistics();

                // better to work with percentile rather than strict limit, but for the purpose of the test
                if(stats?.CurrentEntryCount >= _sizeLimit)
                {
                    ForceEvictLRUCachedItem();
                }

                _memoryCache.Set(key, type, cacheEntryOptions);

            }
            finally
            {
                _connectionLock.Release();
            }
        }

        /// <summary>
        /// According to Microsoft docs: https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-7.0
        /// MemoryCache.Compact attempts to remove the specified percentage of the cache in the following order:
        /// All expired items.
        /// Items by priority. Lowest priority items are removed first.
        /// Least recently used objects.
        /// Items with the earliest absolute expiration.
        /// Items with the earliest sliding expiration.
        ///
        /// Therefore we can piggy back of Compact and not introduce anything manual.
        /// </summary>
        private void ForceEvictLRUCachedItem()
        {
            // get the percentile for 1 item. which will be evicted
            var percentileToRemove = (double)1 / _sizeLimit * 100;
            _memoryCache.Compact(percentileToRemove);
        }

        /// <summary>
        /// Once the item has been evicted one of the the requirement is to notify consumer this has been actioned.
        /// </summary>
        /// <param name="key">The key which was evicted.</param>
        /// <param name="value">The value that was evicted.</param>
        /// <param name="reason">The reason for eviction.</param>
        /// <param name="state">The state.</param>
        private void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            // from here you can publish the a message on to service bus, or upadate a database record to
            // state a cache item with specific key has been evicted.
        }
    }

    public interface IFinbourneCacheService
    {
        T Get<T>(string key);

        Task Set<T>(string key, T type);

        //Task<IEnumerable<T>> GetCollection<T>(string key);
    }
}
