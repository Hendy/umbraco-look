using Lucene.Net.Search;
using System;
using System.Web;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// used to group all vars required by the search parsing methods
    /// </summary>
    internal class ParsingContext
    {
        internal BooleanQuery Query { get; } = new BooleanQuery();

        internal Filter Filter { get; set; }

        internal Sort Sort { get; set; }

        internal Func<string, IHtmlString> GetHighlight { get; set;  } = x => null;

        internal Func<int, double?> GetDistance { get; set;  } = x => null;

        /// <summary>
        /// flag to indicate whether a query clause was added
        /// </summary>
        internal bool HasQuery { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        internal ParsingContext()
        {
        }

        /// <summary>
        /// wrapper
        /// </summary>
        /// <param name="clause"></param>
        internal void QueryAdd(BooleanClause clause)
        {
            this.HasQuery = true;
            this.Query.Add(clause);
        }

        /// <summary>
        /// wrapper
        /// </summary>
        /// <param name="query"></param>
        /// <param name="occur"></param>
        internal void QueryAdd(Query query, BooleanClause.Occur occur)
        {
            this.HasQuery = true;
            this.Query.Add(query, occur);
        }
    }
}
