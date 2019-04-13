using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get the name indexer for a specified index
        /// </summary>
        /// <param name="indexerName">supplied so we can look for a index specific indexer, else fall back to a common default</param>
        /// <returns></returns>
        internal static Func<IndexingContext, string> GetNameIndexer(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).NameIndexer 
                ?? LookService.Instance._nameIndexer;
        }
    }
}
