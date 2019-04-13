using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get the global BeforeIndexing method
        /// </summary>
        /// <returns></returns>
        internal static Action<IndexingContext> GetBeforeIndexing()
        {
            return LookService.Instance._beforeIndexing                            // default
               ?? new Action<IndexingContext>(x => { });                           // not set
        }

        /// <summary>
        /// Get the BeforeIndexing method for a specifed index
        /// </summary>
        /// <param name="indexerName"></param>
        /// <returns></returns>
        internal static Action<IndexingContext> GetBeforeIndexing(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).BeforeIndexing  // indexer specific
                ?? new Action<IndexingContext>(x => { });                           // not set
        }
    }
}
