using Examine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Look.Models
{
    public class ExamineResults : ISearchResults
    {
        /// <summary>
        /// The wrapped enumerable
        /// </summary>
        private IEnumerable<SearchResult> _searchResults;

        /// <summary>
        /// 
        /// </summary>
        public int TotalItemCount { get; }

        /// <summary>
        /// empty constuctor to return empty examine result
        /// </summary>
        internal ExamineResults()
        {
            this.TotalItemCount = 0;
            this._searchResults = Enumerable.Empty<SearchResult>();
        }

        /// <summary>
        /// Internal constructor used to pass in all required data
        /// </summary>
        internal ExamineResults(int totalItemCount, IEnumerable<SearchResult> searchResults)
        {
            this.TotalItemCount = totalItemCount;
            this._searchResults = searchResults;
        }

        public IEnumerator<SearchResult> GetEnumerator()
        {
            return this._searchResults.GetEnumerator();
        }

        public IEnumerable<SearchResult> Skip(int skip)
        {
            return this._searchResults.Skip(skip);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
