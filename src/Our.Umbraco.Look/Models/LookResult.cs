using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Look.Models
{
    public class LookResult : IEnumerable<LookMatch>
    {
        /// <summary>
        /// return a new empty LookResult model
        /// </summary>
        internal static LookResult Empty => new LookResult();

        /// <summary>
        /// wrapped enumerable
        /// </summary>
        private IEnumerable<LookMatch> _lookMatches;

        /// <summary>
        /// Expected total number of LookMatch objects in the enumerable
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// 
        /// </summary>
        public Facet[] Facets { get; }

        /// <summary>
        /// Constructor (for populated)
        /// </summary>
        /// <param name="lookMatches"></param>
        /// <param name="total"></param>
        internal LookResult(IEnumerable<LookMatch> lookMatches, int total, Facet[] facets)
        {
            this._lookMatches = lookMatches;
            this.Total = total;
            this.Facets = facets ?? new Facet[] { };
        }

        /// <summary>
        /// Constructor (for empty)
        /// </summary>
        private LookResult()
        {
            this._lookMatches = Enumerable.Empty<LookMatch>();
            this.Total = 0;
            this.Facets = new Facet[] { };
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
