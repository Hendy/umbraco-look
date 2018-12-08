using Examine.SearchCriteria;
using Our.Umbraco.Look.Services;
using System.Linq;

namespace Our.Umbraco.Look.Models
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
                    var source = this._compiled.Source; // must exist, else it couldn't have been compiled

                    // if the current query hasn't changed since being compiled
                    if (source.RawQuery == this.RawQuery                        
                        && ((source.NodeQuery == null && this.NodeQuery == null) || this.NodeQuery != null && this.NodeQuery.Equals(source.NodeQuery))
                        && ((source.NameQuery == null && this.NameQuery == null) || this.NameQuery != null && this.NameQuery.Equals(source.NameQuery))
                        && ((source.DateQuery == null && this.DateQuery == null) || this.DateQuery != null && this.DateQuery.Equals(source.DateQuery))
                        && ((source.TextQuery == null && this.TextQuery == null) || this.TextQuery != null && this.TextQuery.Equals(source.TextQuery))
                        && ((source.TagQuery == null && this.TagQuery == null) || this.TagQuery != null && this.TagQuery.Equals(source.TagQuery))
                        && ((source.LocationQuery == null && this.LocationQuery == null) || this.LocationQuery != null && this.LocationQuery.Equals(source.LocationQuery)))
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
        public void ApplyFacet(Facet facet)
        {
            if (facet != null)
            {
                this._compiled = null;

                if (this.TagQuery == null)
                {
                    this.TagQuery = new TagQuery();
                }

                if (this.TagQuery.All == null)
                {
                    this.TagQuery.All = new LookTag[] { facet.Tag };
                }
                else
                {
                    this.TagQuery.All = this.TagQuery.All.Concat(new LookTag[] { facet.Tag }).ToArray();
                }
            }
        }

        /// <summary>
        /// Perform the query - this is a shortcut to and does the same thing as LookService.Query(this)
        /// </summary>
        /// <returns></returns>
        public LookResult Query()
        {
            return LookService.Query(this);
        }

        internal LookQuery Clone()
        {
            var clone = new LookQuery();

            clone.RawQuery = this.RawQuery;
            clone.NodeQuery = this.NodeQuery?.Clone();
            clone.NameQuery = this.NameQuery?.Clone();
            clone.DateQuery = this.DateQuery?.Clone();
            clone.TextQuery = this.TextQuery?.Clone();
            clone.TagQuery = this.TagQuery?.Clone();
            clone.LocationQuery = this.LocationQuery?.Clone();

            return clone;
        }
    }
}
