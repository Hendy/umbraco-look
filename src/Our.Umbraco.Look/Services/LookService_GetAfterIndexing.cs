using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Action<IndexingContext> GetAfterIndexing(string indexerName)
        {
            return LookService.GetIndexerConfiguration(indexerName).AfterIndexing
                ?? LookService.Instance._afterIndexing 
                ?? new Action<IndexingContext>(x => { });
        }
    }
}
