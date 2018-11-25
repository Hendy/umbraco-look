using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Model returned as a response to a Look query
    /// </summary>
    public class LookResult : IEnumerable<LookMatch>
    {
        /// <summary>
        /// The wrapped enumerable
        /// </summary>
        private IEnumerable<LookMatch> _lookMatches;

        /// <summary>
        /// Expected total number of results in the enumerable
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// 
        /// </summary>
        public Facet[] Facets { get; }

        /// <summary>
        /// When true, indicates the Look Query was parsed and executed correctly
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// returns the compiled look query, (optimised for being returned back into the query)
        /// </summary>
        public LookQuery CompiledQuery { get; }

        /// <summary>
        /// Constructor for populated result
        /// </summary>
        /// <param name="lookMatches"></param>
        /// <param name="total"></param>
        /// <param name="facets"></param>
        /// <param name="lookQuery"></param>
        internal LookResult(IEnumerable<LookMatch> lookMatches, int total, Facet[] facets, LookQuery lookQuery)
        {            
            this._lookMatches = lookMatches;
            this.Total = total;
            this.Facets = facets ?? new Facet[] { };
            this.Success = true;
            this.CompiledQuery = lookQuery;
        }

        /// <summary>
        /// Constructor for an empty result
        /// </summary>
        internal LookResult(LookQuery lookQuery)
        {
            this._lookMatches = Enumerable.Empty<LookMatch>();
            this.Total = 0;
            this.Facets = new Facet[] { };
            this.Success = true;
            this.CompiledQuery = lookQuery;
        }

        /// <summary>
        /// Constructor for an error result
        /// </summary>
        /// <param name="failureMessage">Failure message to debug log</param>
        internal LookResult(string failureMessage)
        {
            this._lookMatches = Enumerable.Empty<LookMatch>();
            this.Total = 0;
            this.Facets = new Facet[] { };
            this.Success = false;

            LogHelper.Debug(typeof(LookResult), failureMessage);
        }

        public IEnumerator<LookMatch> GetEnumerator()
        {
            return this._lookMatches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
