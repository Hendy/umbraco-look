using System;
using System.Runtime.Caching;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing location
        /// </summary>
        /// <param name="locationIndexer">Your custom location indexing function</param>
        internal static void SetLocationIndexer(Func<IndexingContext, Location> locationIndexer)
        {
            MemoryCache.Default.Remove(LookConstants.LocationIndexerCacheKey);

            if (LookService.Instance._locationIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Location indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Location indexing function replaced");
            }

            LookService.Instance._locationIndexer = locationIndexer;
        }
    }
}
