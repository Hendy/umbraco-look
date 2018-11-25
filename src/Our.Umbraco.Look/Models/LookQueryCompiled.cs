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
        internal BooleanQuery Query { get; }

        internal Filter Filter { get; }

        internal Sort Sort { get; }

        internal Func<string, IHtmlString> GetHighlight { get; }

        internal Func<int, double?> GetDistance { get; }

        internal LookQueryCompiled(
                    BooleanQuery query, 
                    Filter filter, 
                    Sort sort, 
                    Func<string, IHtmlString> getHighlight, 
                    Func<int, double?> getDistance)
        {
            this.Query = query;
            this.Filter = filter;
            this.Sort = sort;
            this.GetHighlight = getHighlight;
            this.GetDistance = getDistance;
        }
    }
}
