using Newtonsoft.Json;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class RebuildViewData
    { 
        [JsonProperty("IndexName")]
        public string IndexName { get; set; }
    }
}
