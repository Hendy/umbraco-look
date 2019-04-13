using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, string> GetTextIndexer(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).TextIndexer
                ?? LookService.Instance._defaultTextIndexer;
        }
    }
}
