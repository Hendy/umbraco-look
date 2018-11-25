namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Model used to specify the search query criteria for a LookService.Query
    /// </summary>
    public class LookQuery
    {
        /// <summary>
        /// (Optional) set a raw Lucene query
        /// </summary>
        public string RawQuery { get; set; }

        /// <summary>
        /// (Optional) set search critera for Umbraco aliases and/or ids
        /// </summary>
        public NodeQuery NodeQuery { get; set; } = new NodeQuery();

        /// <summary>
        /// (Optional) set search critera for the custom name field
        /// </summary>
        public NameQuery NameQuery { get; set; } = new NameQuery();

        /// <summary>
        /// (Optional) set a before and/or an after date for the custom date field
        /// </summary>
        public DateQuery DateQuery { get; set; } = new DateQuery();

        /// <summary>
        /// (Optional) set text search criteria for the custom text field
        /// </summary>
        public TextQuery TextQuery { get; set; } = new TextQuery();

        /// <summary>
        /// (Optional) set tag query criteria for the custom tag field
        /// </summary>
        public TagQuery TagQuery { get; set; } = new TagQuery();

        /// <summary>
        /// (Optional) set geospatial search criteria
        /// </summary>
        public LocationQuery LocationQuery { get; set; } = new LocationQuery();

        /// <summary>
        /// (Optional) specify the field to sort on
        /// </summary>
        public SortOn SortOn { get; set; } = SortOn.Score;

        /// <summary>
        /// Name of searcher (index) to use
        /// </summary>
        internal string SearcherName { get; private set; }

        /// <summary>
        /// Model representing collection of properties that have been processed from the raw LookQuery properties, and ready for a lucene query
        /// </summary>
        internal LookQueryCompiled Compiled { get; set; } = null;

        /// <summary>
        /// Create a new Look query using the default Examine searcher (usually "ExternalSearcher", see config/ExamineSettings.config)
        /// </summary>
        public LookQuery()
        {
        }

        /// <summary>
        /// Create a new Look query using the specified Exmaine searcher (see config/ExamineSettings.config for available searchers)
        /// </summary>
        /// <param name="searcherName">The name of the Examine searcher to use</param>
        public LookQuery(string searcherName)
        {
            this.SearcherName = searcherName;
        }
    }
}
