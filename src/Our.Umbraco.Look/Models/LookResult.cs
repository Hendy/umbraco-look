using Examine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Model returned as a response to a Look query
    /// </summary>
    public class LookResult : ISearchResults
    {
        /// <summary>
        /// The wrapped enumerable
        /// </summary>
        private IEnumerable<LookMatch> _lookMatches;

        /// <summary>
        /// Expected total number of results in the enumerable
        /// </summary>
        public int TotalItemCount { get; } // TODO: rename to TotalItemCount (to match that in any ExamineResults)

        /// <summary>
        /// 
        /// </summary>
        public Facet[] Facets { get; }

        /// <summary>
        /// Return the results of the query in the same way as Examine does
        /// </summary>
        public ISearchResults ExamineResults { get; }

        /// <summary>
        /// When true, indicates the Look Query was parsed and executed correctly
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Constructor for populated result
        /// </summary>
        /// <param name="lookMatches"></param>
        /// <param name="total"></param>
        /// <param name="facets"></param>
        /// <param name="examineResults"></param>
        internal LookResult(IEnumerable<LookMatch> lookMatches, int total, Facet[] facets, IEnumerable<SearchResult> examineResults)
        {            
            this._lookMatches = lookMatches;
            this.TotalItemCount = total;
            this.Facets = facets ?? new Facet[] { };
            this.ExamineResults = new ExamineResults(total, examineResults);
            this.Success = true;
        }

        /// <summary>
        /// Constructor for an empty (success) result
        /// </summary>
        internal LookResult()
        {
            this._lookMatches = Enumerable.Empty<LookMatch>();
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
            this._lookMatches = Enumerable.Empty<LookMatch>();
            this.TotalItemCount = 0;
            this.Facets = new Facet[] { };
            this.Success = false;

            LogHelper.Debug(typeof(LookResult), loggingMessage);
        }

        public IEnumerable<SearchResult> Skip(int skip)
        {
            return this._lookMatches.Skip(skip);
        }

        public IEnumerator<SearchResult> GetEnumerator()
        {
            return this._lookMatches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
