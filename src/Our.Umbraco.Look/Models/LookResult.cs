using System.Collections;
using System.Collections.Generic;

namespace Our.Umbraco.Look.Models
{
    public class LookResult : IEnumerable<LookMatch>
    {
        private IEnumerable<LookMatch> _lookMatches;

        /// <summary>
        /// Expected total number of LookMatch objects in the enumerable
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lookMatches"></param>
        /// <param name="total"></param>
        internal LookResult(IEnumerable<LookMatch> lookMatches, int total)
        {
            this._lookMatches = lookMatches;
            this.Total = total;
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
