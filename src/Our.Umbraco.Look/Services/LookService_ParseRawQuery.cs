using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Our.Umbraco.Look.Models;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookQuery">The query to parse</param>
        /// <param name="parsingContext"></param>
        private static void ParseRawQuery(LookQuery lookQuery, ParsingContext parsingContext)
        {
            if (!string.IsNullOrWhiteSpace(lookQuery.RawQuery))
            {
                parsingContext.QueryAdd(
                        new QueryParser(Lucene.Net.Util.Version.LUCENE_29, null, lookQuery.SearchingContext.Analyzer).Parse(lookQuery.RawQuery),
                        BooleanClause.Occur.MUST);
            }
        }
    }
}
