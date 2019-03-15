using System;
using System.Runtime.Caching;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, DateTime?> GetDateIndexer()
        {
            var dateIndexer = MemoryCache.Default.Get(LookConstants.DateIndexerCacheKey) as Func<IndexingContext, DateTime?>;

            if (dateIndexer == null)
            {
                dateIndexer = LookService.Instance._dateIndexer;

                if (dateIndexer != null)
                {
                    MemoryCache.Default.Set(LookConstants.DateIndexerCacheKey, dateIndexer, new CacheItemPolicy());
                }
            }

            return dateIndexer;
        }
    }
}
