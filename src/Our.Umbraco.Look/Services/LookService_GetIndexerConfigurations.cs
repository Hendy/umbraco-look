using System.Collections.Generic;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Dictionary<string, IndexerConfiguration> GetIndexerConfigurations()
        {
            return LookService.Instance._indexerConfigurations;
        }
    }
}
