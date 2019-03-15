using System;
using System.Runtime.Caching;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, LookTag[]> GetTagIndexer()
        {
            var tagIndexer = MemoryCache.Default.Get(LookConstants.TagIndexerCacheKey) as Func<IndexingContext, LookTag[]>;

            if (tagIndexer == null)
            {
                tagIndexer = LookService.Instance._tagIndexer;

                if (tagIndexer != null)
                {
                    MemoryCache.Default.Set(LookConstants.TagIndexerCacheKey, tagIndexer, new CacheItemPolicy());
                }
            }

            return tagIndexer;
        }
    }
}
