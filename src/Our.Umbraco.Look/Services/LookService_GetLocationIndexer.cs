using System;
using System.Runtime.Caching;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, Location> GetLocationIndexer()
        {
            var locationIndexer = MemoryCache.Default.Get(LookConstants.LocationIndexerCacheKey) as Func<IndexingContext, Location>;

            if (locationIndexer == null)
            {
                locationIndexer = LookService.Instance._locationIndexer;

                if (locationIndexer != null)
                {
                    MemoryCache.Default.Set(LookConstants.LocationIndexerCacheKey, locationIndexer, new CacheItemPolicy());
                }
            }

            return locationIndexer;
        }
    }
}
