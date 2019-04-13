using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get global AfterIndexing
        /// </summary>
        /// <returns></returns>
        internal static Action<IndexingContext> GetAfterIndexing()
        {
            return LookService.Instance._afterIndexing
                ?? new Action<IndexingContext>(x => { });
        }

        /// <summary>
        /// Get indexer specific AfterIndexing
        /// </summary>
        /// <param name="indexerName"></param>
        /// <returns></returns>
        internal static Action<IndexingContext> GetAfterIndexing(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).AfterIndexing
                ?? new Action<IndexingContext>(x => { });
        }
    }
}
