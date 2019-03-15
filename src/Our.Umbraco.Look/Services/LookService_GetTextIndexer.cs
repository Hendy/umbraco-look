using System;
using System.Runtime.Caching;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, string> GetTextIndexer()
        {
            var textIndexer = MemoryCache.Default.Get(LookConstants.TextIndexerCacheKey) as Func<IndexingContext, string>;

            if (textIndexer == null)
            {
                textIndexer = LookService.Instance._textIndexer;

                if (textIndexer != null)
                {
                    MemoryCache.Default.Set(LookConstants.TextIndexerCacheKey, textIndexer, new CacheItemPolicy());
                }
            }

            return textIndexer;
        }
    }
}
