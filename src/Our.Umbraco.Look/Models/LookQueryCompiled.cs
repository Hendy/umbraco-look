using Lucene.Net.Search;
using System;
using System.Web;

namespace Our.Umbraco.Look.Models
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
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parsingContext"></param>
        internal LookQueryCompiled(LookQuery source, ParsingContext parsingContext)
        {
            this.Source = source.Clone();
            this.Query = parsingContext.Query;
            this.Filter = parsingContext.Filter;
            this.Sort = parsingContext.Sort ?? new Sort(SortField.FIELD_SCORE);
            this.GetHighlight = parsingContext.GetHighlight;
            this.GetDistance = parsingContext.GetDistance;
        }
    }
}
