using System;
using System.Runtime.Caching;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Set a custom function to execute at index-time, which will return a string value for the name field (or null to not index)
        /// </summary>
        /// <param name="nameIndexer">The custom name indexing function</param>
        internal static void SetNameIndexer(Func<IndexingContext, string> nameIndexer)
        {
            MemoryCache.Default.Remove(LookConstants.NameIndexerCahceKey);

            if (LookService.Instance._nameIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Name indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Name indexing function replaced");
            }

            LookService.Instance._nameIndexer = nameIndexer;
        }
    }
}
