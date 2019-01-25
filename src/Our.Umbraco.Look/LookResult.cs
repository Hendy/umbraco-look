using Examine;
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
        /// <summary>
        /// Expected total number of results in the enumerable
        /// </summary>
        public int TotalItemCount { get; }

        /// <summary>
        /// Get the results enumerable with the LookMatch objects
        /// </summary>
        public IEnumerable<LookMatch> Matches { get; }

        /// <summary>
        /// Always returned as an array (which may be empty)
        /// </summary>
        public Facet[] Facets { get; }

        /// <summary>
        /// When true, indicates the Look Query was parsed, contained a query clause and was executed correctly
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Constructor for populated result
        /// </summary>
        /// <param name="lookMatches"></param>
        /// <param name="total"></param>
        /// <param name="facets"></param>
        internal LookResult(IEnumerable<LookMatch> lookMatches, int total, Facet[] facets)
        {            
            this.Matches = lookMatches;
            this.TotalItemCount = total;
            this.Facets = facets ?? new Facet[] { };
            this.Success = true;
        }

        /// <summary>
        /// Constructor for an empty (success) result
        /// </summary>
        internal LookResult()
        {
            this.Matches = Enumerable.Empty<LookMatch>();
            this.TotalItemCount = 0;
            this.Facets = new Facet[] { };
            this.Success = true;
        }

        /// <summary>
        /// Constructor for an empty (failed) result with message for debug logging
        /// </summary>
        /// <param name="loggingMessage">Message to debug log</param>
        internal LookResult(string loggingMessage)
        {
            this.Matches = Enumerable.Empty<LookMatch>();
            this.TotalItemCount = 0;
            this.Facets = new Facet[] { };
            this.Success = false;

            LogHelper.Debug(typeof(LookResult), loggingMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        public IEnumerable<SearchResult> Skip(int skip)
        {
            return this.Matches.Skip(skip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SearchResult> GetEnumerator()
        {
            return this.Matches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
