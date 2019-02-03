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
        public string SearcherName { get; set; }

        /// <summary>
        /// Searcher Description
        /// </summary>
        public string SearcherDescription { get; set; }

        /// <summary>
        /// Examine "Examine (active or inactive)", "Look"
        /// </summary>
        public string SearcherType { get; set; }

        /// <summary>
        /// Css icon name
        /// </summary>
        public string Icon { get; set; }
    }
}
