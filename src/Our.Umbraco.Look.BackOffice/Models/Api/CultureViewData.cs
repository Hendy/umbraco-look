using Newtonsoft.Json;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class CultureViewData
    {
        [JsonProperty("cultureName")]
        public string CultureName { get; set; }
    }
}
