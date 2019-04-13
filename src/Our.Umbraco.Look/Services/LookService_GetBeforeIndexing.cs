using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get the BeforeIndexing method for a specifed index
        /// </summary>
        /// <param name="indexerName"></param>
        /// <returns></returns>
        internal static Action<IndexingContext> GetBeforeIndexing(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).BeforeIndexing  // indexer specific
                ?? LookService.Instance._beforeIndexing                             // default
                ?? new Action<IndexingContext>(x => { });                           // not set
        }
    }
}
