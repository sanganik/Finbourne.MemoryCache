using Finbourne.Cache.Component.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Finbourne.Cache.Component
{
    public class FinbourneMemoryCache : IFinbourneMemoryCache
    {
        public readonly long _sizeLimit;

        public FinbourneMemoryCache()
        {
            _sizeLimit = long.Parse(ConfigurationManager.AppSettings["Finbourne.MemoryCache.SizeLimit"]);
        }

        public long SizeLimit => _sizeLimit;

        public MemoryCache Get()
        {
            // manage custom size limit which is configurable
            return new MemoryCache(
                        new MemoryCacheOptions
                        {
                            SizeLimit = _sizeLimit,
                            TrackStatistics = true
                        });

        }
    }

    public interface IFinbourneMemoryCache
    {
        long SizeLimit { get; }
        MemoryCache Get();
    }
}
