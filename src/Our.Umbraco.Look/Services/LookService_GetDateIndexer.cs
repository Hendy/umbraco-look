using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, DateTime?> GetDateIndexer(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).DateIndexer
                ?? LookService.Instance._defaultDateIndexer;
        }
    }
}
