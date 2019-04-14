using Newtonsoft.Json;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class FiltersResult
    {
        //[JsonProperty("aliasOptions")]
        //public AliasOption[] AliasOptions { get; set; }

        [JsonProperty("documentTypeAliases")]
        public string[] DocumentTypeAliases { get; set; }

        [JsonProperty("mediaTypeAliases")]
        public string[] MediaTypeAliases { get; set; }

        [JsonProperty("memberTypeAliases")]
        public string[] MemberTypeAliases { get; set; }

        //// TODO: make subsclass for model suitable to renering in dropdown -each item, name, value, enabled & sorted in renering order (grouped by type)
        //public class AliasOption
        //{
        //}
    }
}
