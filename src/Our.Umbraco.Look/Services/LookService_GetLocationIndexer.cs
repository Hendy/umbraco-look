using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, Location> GetLocationIndexer(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).LocationIndexer
                ?? LookService.Instance._defaultLocationIndexer;
        }
    }
}
