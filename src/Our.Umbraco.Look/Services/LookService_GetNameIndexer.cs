using System;
using System.Runtime.Caching;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, string> GetNameIndexer()
        {
            var nameIndexer = MemoryCache.Default.Get(LookConstants.NameIndexerCahceKey) as Func<IndexingContext, string>;

            if (nameIndexer == null)
            {
                nameIndexer = LookService.Instance._nameIndexer;

                if (nameIndexer != null)
                {
                    MemoryCache.Default.Set(LookConstants.NameIndexerCahceKey, nameIndexer, new CacheItemPolicy());
                }
            }

            return nameIndexer;
        }
    }
}
