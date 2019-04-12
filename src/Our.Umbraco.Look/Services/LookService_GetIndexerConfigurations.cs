using System.Collections.Generic;
using System.Linq;

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
