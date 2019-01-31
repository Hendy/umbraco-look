namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    /// <summary>
    /// Response POCO for the ApiController.GetSearcherDetails() method
    /// </summary>
    public class SearcherDetailsResponse
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
    }
}
