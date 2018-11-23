namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Model used to specify the search query criteria
    /// </summary>
    public class LookQuery
    {
        /// <summary>
        /// Set raw Lucene query criteria
        /// </summary>
        public string RawQuery { get; set; }

        /// <summary>
        /// Specify node query criteria
        /// </summary>
        public NodeQuery NodeQuery { get; set; } = new NodeQuery();

        ///// <summary>
        ///// 
        ///// </summary>
        //public NameQuery NameQuery { get; set; } = new NameQuery();

        /// <summary>
        /// Set a before and/or an after date
        /// </summary>
        public DateQuery DateQuery { get; set; } = new DateQuery();

        /// <summary>
        /// Set search text and specify whether text highlighting should be returned
        /// </summary>
        public TextQuery TextQuery { get; set; } = new TextQuery();

        /// <summary>
        /// Set tag query criteria
        /// </summary>
        public TagQuery TagQuery { get; set; } = new TagQuery();

        /// <summary>
        /// Set geospatial query criteria
        /// </summary>
        public LocationQuery LocationQuery { get; set; } = new LocationQuery();

        /// <summary>
        /// Specify the field to sort on
        /// </summary>
        public SortOn SortOn { get; set; } = SortOn.Score;

        /// <summary>
        /// Name of searcher (index) to use (the context for the query properties as it were)
        /// </summary>
        internal string SearcherName { get; private set; }

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
