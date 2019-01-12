using Examine.SearchCriteria;
using Our.Umbraco.Look.Models;
using System.Linq;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Model used to specify the search query criteria for a LookService.Query
    /// </summary>
    public class LookQuery
    {
        /// <summary>
        /// 
        /// </summary>
        private LookQueryCompiled _compiled = null;

        /// <summary>
        /// Property to override the default RequestFields behaviour
        /// </summary>
        public RequestFields? RequestFields { get; set; } = null;

        /// <summary>
        /// (Optional) set a raw Lucene query
        /// </summary>
        public string RawQuery { get; set; }

        /// <summary>
        /// (Optional) set a Examine query
        /// </summary>
        public ISearchCriteria ExamineQuery { get; set; }

        /// <summary>
        /// (Optional) set search critera for Umbraco aliases and/or ids
        /// </summary>
        public NodeQuery NodeQuery { get; set; }

        /// <summary>
        /// (Optional) set search critera for the custom name field
        /// </summary>
        public NameQuery NameQuery { get; set; }

        /// <summary>
        /// (Optional) set a before and/or an after date for the custom date field
        /// </summary>
        public DateQuery DateQuery { get; set; }

        /// <summary>
        /// (Optional) set text search criteria for the custom text field
        /// </summary>
        public TextQuery TextQuery { get; set; }

        /// <summary>
        /// (Optional) set tag query criteria for the custom tag field
        /// </summary>
        public TagQuery TagQuery { get; set; }

        /// <summary>
        /// (Optional) set geospatial search criteria
        /// </summary>
        public LocationQuery LocationQuery { get; set; }

        /// <summary>
        /// (Optional) specify the field to sort on
        /// </summary>
        public SortOn SortOn { get; set; } = SortOn.Score;

        /// <summary>
        /// Name of searcher (index) to use
        /// </summary>
        internal string SearcherName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal SearchingContext SearchingContext { get; set; }

        /// <summary>
        /// Model representing collection of properties that have been processed from the raw LookQuery properties, and ready for a lucene query
        /// </summary>
        internal LookQueryCompiled Compiled
        {
            get
            {
                if (this._compiled != null)
                {
                    if (this.Equals(this._compiled.Source)) // .Source must exist, else this couldn't have been compiled
                    {
                        return this._compiled;
                    }

                    this._compiled = null; // remove compiled as query has changed
                }

                return null;
            }
            set
            {
                this._compiled = value;
            }
        }

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

        /// <summary>
        /// internal constructor for unit testing, allows tests to supply context bypassing Umbraco Examine
        /// </summary>
        internal LookQuery(SearchingContext searchingContext)
        {
            this.SearchingContext = searchingContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facet"></param>
        /// <returns></returns>
        public LookQuery ApplyFacet(Facet facet)
        {
            if (facet != null)
            {
                // TODO: check facet originated from this query

                this._compiled = null;

                if (this.TagQuery == null)
                {
                    this.TagQuery = new TagQuery();
                }

                if (this.TagQuery.All == null)
                {
                    this.TagQuery.All = facet.Tags;
                }
                else
                {
                    this.TagQuery.All = this.TagQuery.All.Concat(facet.Tags).ToArray();
                }
            }

            return this;
        }

        /// <summary>
        /// Perform the query - this is a shortcut to and does the same thing as LookService.RunQuery(this)
        /// </summary>
        /// <returns></returns>
        public LookResult Run()
        {
            return LookService.RunQuery(this);
        }

        /// <summary>
        /// Compare two LookQuery objects to see if their search criteria match and are using same searcher
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            LookQuery lookQuery = obj as LookQuery;

            return
                lookQuery != null
                && lookQuery.RequestFields == this.RequestFields
                && lookQuery.RawQuery == this.RawQuery
                && ((lookQuery.NodeQuery == null && this.NodeQuery == null) || this.NodeQuery != null && this.NodeQuery.Equals(lookQuery.NodeQuery))
                && ((lookQuery.NameQuery == null && this.NameQuery == null) || this.NameQuery != null && this.NameQuery.Equals(lookQuery.NameQuery))
                && ((lookQuery.DateQuery == null && this.DateQuery == null) || this.DateQuery != null && this.DateQuery.Equals(lookQuery.DateQuery))
                && ((lookQuery.TextQuery == null && this.TextQuery == null) || this.TextQuery != null && this.TextQuery.Equals(lookQuery.TextQuery))
                && ((lookQuery.TagQuery == null && this.TagQuery == null) || this.TagQuery != null && this.TagQuery.Equals(lookQuery.TagQuery))
                && ((lookQuery.LocationQuery == null && this.LocationQuery == null) || this.LocationQuery != null && this.LocationQuery.Equals(lookQuery.LocationQuery))
                && lookQuery.SortOn == this.SortOn
                && lookQuery.SearcherName == this.SearcherName;
        }

        internal LookQuery Clone()
        {
            var clone = new LookQuery();

            clone.RequestFields = this.RequestFields;
            clone.RawQuery = this.RawQuery;
            clone.NodeQuery = this.NodeQuery?.Clone();
            clone.NameQuery = this.NameQuery?.Clone();
            clone.DateQuery = this.DateQuery?.Clone();
            clone.TextQuery = this.TextQuery?.Clone();
            clone.TagQuery = this.TagQuery?.Clone();
            clone.LocationQuery = this.LocationQuery?.Clone();
            clone.SortOn = this.SortOn;
            clone.SearcherName = this.SearcherName;
            clone.SearchingContext = this.SearchingContext; // required for unit tests

            return clone;
        }
    }
}
