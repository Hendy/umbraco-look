using Newtonsoft.Json;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    /// <summary>
    /// Model for view when a searcher is selected in the tree
    /// </summary>
    public class SearcherViewData
    {
        /// <summary>
        /// Searcher Name
        /// </summary>
        [JsonProperty("searcherName")]
        public string SearcherName { get; set; }

        /// <summary>
        /// Searcher Description
        /// </summary>
        [JsonProperty("searcherDescription")]
        public string SearcherDescription { get; set; }

        /// <summary>
        /// Examine "Examine (active or inactive)", "Look"
        /// </summary>
        [JsonProperty("searcherType")]
        public string SearcherType { get; set; }

        /// <summary>
        /// Css icon name
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Is Look enabled for thie searcher ? (will be true for a Look indexer, but can opt out from Examine indexes)
        /// </summary>
        [JsonProperty("lookIndexingEnabled")]
        public bool LookIndexingEnabled { get; set; }
    }
}
