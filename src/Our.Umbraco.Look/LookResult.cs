using Examine;
using Lucene.Net.Search;
using Our.Umbraco.Look.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Response model for a LookQuery
    /// </summary>
    public class LookResult : ISearchResults
    {
        private LookQuery _lookQuery;

        private TopDocs _topDocs;

        /// <summary>
        /// Flag used to indicate that _lookQuery, and _topDocs are populated (and results expected)
        /// </summary>
        private bool _hasMatches = false;

        /// <summary>
        /// The total number of results that could be returned from Lucene
        /// </summary>
        public int TotalItemCount { get; } = 0;

        /// <summary>
        /// Get the results enumerable with the LookMatch objects
        /// </summary>
        public IEnumerable<LookMatch> Matches
        {
            get
            {
                if (this._hasMatches)
                {
                    return LookService
                            .GetLookMatches(
                                this._lookQuery.SearcherName,
                                this._lookQuery.SearchingContext.IndexSearcher,
                                this._topDocs.ScoreDocs,
                                this._lookQuery.RequestFields,
                                this._lookQuery.Compiled.GetHighlight,
                                this._lookQuery.Compiled.GetDistance);
                }

                return Enumerable.Empty<LookMatch>();
            }
        }

        /// <summary>
        /// Always returned as an array (which may be empty)
        /// </summary>
        public Facet[] Facets { get; } = new Facet[] { };

        /// <summary>
        /// When true, indicates the Look Query was parsed, contained a query clause and was executed correctly
        /// </summary>
        public bool Success { get; private set; } = false;

        /// <summary>
        /// Default constructor for empty or error result
        /// </summary>
        private LookResult()
        {
        }

        /// <summary>
        /// Constructor with results
        /// </summary>
        /// <param name="lookQuery"></param>
        /// <param name="topDocs"></param>
        /// <param name="facets"></param>
        internal LookResult(LookQuery lookQuery, TopDocs topDocs, Facet[] facets)
        {
            this._lookQuery = lookQuery;
            this._topDocs = topDocs;
            this._hasMatches = true; // this constructor is only called when there are matches
            this.TotalItemCount = topDocs.TotalHits;
            this.Facets = facets;
            this.Success = true;
        }

        /// <summary>
        /// Use this Skip for performace (and then cast each result to a LookMatch)
        /// This skip method, performs the skip on the Lucene results array before each is inflated into a LookMatch
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        public IEnumerable<SearchResult> Skip(int skip)
        {
            if (this._hasMatches && skip > 0)
            {
                var scoreDocs = this._topDocs.ScoreDocs.Skip(skip).ToArray();

                return LookService
                        .GetLookMatches(
                            this._lookQuery.SearcherName,
                            this._lookQuery.SearchingContext.IndexSearcher,
                            scoreDocs,
                            this._lookQuery.RequestFields,
                            this._lookQuery.Compiled.GetHighlight,
                            this._lookQuery.Compiled.GetDistance);
            }

            return this.Matches; 
        }

        /// <summary>
        /// Helper to call the efficient skip method, but to return each result as a LookMatch (rather then the underlying Examine.SearchResult)
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        public IEnumerable<LookMatch> SkipMatches(int skip)
        {
            return this.Skip(skip).Select(x => (LookMatch)x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SearchResult> GetEnumerator()
        {
            return this.Matches.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns an empty (successful) LookResult
        /// </summary>
        /// <returns></returns>
        internal static LookResult Empty()
        {
            return new LookResult() { Success = true };
        }

        /// <summary>
        /// Return an empty (unsuccessful) LookResult, whilst logging error
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        internal static LookResult Error(string logMessage)
        {
            LogHelper.Debug(typeof(LookResult), logMessage);

            return new LookResult(); // { Success = false };
        }
    }
}
