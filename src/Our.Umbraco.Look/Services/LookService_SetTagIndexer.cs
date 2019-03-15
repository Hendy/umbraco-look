using System;
using System.Runtime.Caching;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing tags
        /// </summary>
        /// <param name="tagIndexer">Your custom tag indexing function</param>
        internal static void SetTagIndexer(Func<IndexingContext, LookTag[]> tagIndexer)
        {
            MemoryCache.Default.Remove(LookConstants.TagIndexerCacheKey);

            if (LookService.Instance._tagIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Tag indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Tag indexing function replaced");
            }
            
            LookService.Instance._tagIndexer = tagIndexer;
        }
    }
}
