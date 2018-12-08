using Lucene.Net.Search;
using System;
using System.Web;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// represents look query search criteria properties processed into objects ready to query Lucene with
    /// </summary>
    internal class LookQueryCompiled
    {
        /// <summary>
        /// The state of the LookQuery at compilation time
        /// </summary>
        internal LookQuery Source { get;  }

        /// <summary>
        /// The built Lucene query
        /// </summary>
        internal BooleanQuery Query { get; }

        /// <summary>
        /// The built Lucene filter
        /// </summary>
        internal Filter Filter { get; }

        /// <summary>
        /// The built Lucene sort
        /// </summary>
        internal Sort Sort { get; }

        /// <summary>
        /// The built function to execute for highlights for each result
        /// </summary>
        internal Func<string, IHtmlString> GetHighlight { get; }

        /// <summary>
        /// The built function to execute for distance results for each function
        /// </summary>
        internal Func<int, double?> GetDistance { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="getHighlight"></param>
        /// <param name="getDistance"></param>
        internal LookQueryCompiled(
                    LookQuery source,
                    BooleanQuery query, 
                    Filter filter, 
                    Sort sort, 
                    Func<string, IHtmlString> getHighlight, 
                    Func<int, double?> getDistance)
        {
            this.Source = source.Clone();
            this.Query = query;
            this.Filter = filter;
            this.Sort = sort;
            this.GetHighlight = getHighlight;
            this.GetDistance = getDistance;
        }
    }
}
