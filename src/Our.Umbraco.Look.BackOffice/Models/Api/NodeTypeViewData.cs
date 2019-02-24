using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class NodeTypeViewData
    {
        ///// <summary>
        ///// Flag to specify Content, Media or Member
        ///// </summary>
        //public PublishedItemType NodeType { get; set; }

        /// <summary>
        /// Css icon name
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Content, Media or Member
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
