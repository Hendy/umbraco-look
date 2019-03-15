using System;
using System.Runtime.Caching;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing date
        /// </summary>
        /// <param name="dateIndexer">Your custom date indexing function</param>
        internal static void SetDateIndexer(Func<IndexingContext, DateTime?> dateIndexer)
        {
            MemoryCache.Default.Remove(LookConstants.DateIndexerCacheKey);

            if (LookService.Instance._dateIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Date indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Date indexing function replaced");
            }
            
            LookService.Instance._dateIndexer = dateIndexer;
        }
    }
}
