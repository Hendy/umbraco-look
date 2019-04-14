using Newtonsoft.Json;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class FiltersResult
    {
        [JsonProperty("aliases")]
        public string[] Aliases { get; set; }

        //[JsonProperty("documentTypeAliases")]
        //public string[] DocumentTypeAliases { get; set; }

        //[JsonProperty("mediaTypeAliases")]
        //public string[] MediaTypeAliases { get; set; }

        //[JsonProperty("memberTypeAliases")]
        //public string[] MemberTypeAliases { get; set; }
    }
}
