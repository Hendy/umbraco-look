using Newtonsoft.Json;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class DetachedViewData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
