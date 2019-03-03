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

        /// <summary>
        /// should default to true when indexing enabled (as default indexing behaviour will be to index the IPublishedContent.Name property)
        /// </summary>
        [JsonProperty("nameIndexerSet")]
        public bool NameIndexerSet { get; set; }

        /// <summary>
        /// should default to true when indexing enabled (as default indexing behaviour will be to index the IPublishedContent.UpdateDate property)
        /// </summary>
        [JsonProperty("dateIndexerSet")]
        public bool DateIndexerSet { get; set; }

        [JsonProperty("textIndexerSet")]
        public bool TextIndexerSet { get; set; }

        [JsonProperty("tagIndexerSet")]
        public bool TagIndexerSet { get; set; }

        [JsonProperty("locationIndexerSet")]
        public bool LocationIndexerSet { get; set; }
    }
}
