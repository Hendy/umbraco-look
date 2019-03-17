using Newtonsoft.Json;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class RebuildViewData
    {
        [JsonProperty("validIndexer")]
        public bool ValidIndexer { get; set; } = false;

        [JsonProperty("indexerName")]
        public string IndexerName { get; set; }
    }
}
