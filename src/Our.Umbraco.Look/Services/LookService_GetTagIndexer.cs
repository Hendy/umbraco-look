using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, LookTag[]> GetTagIndexer(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).TagIndexer
                ?? LookService.Instance._tagIndexer;
        }
    }
}
