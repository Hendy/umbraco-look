using Examine.LuceneEngine.SearchCriteria;
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
        private static void ParseExamineQuery(LookQuery lookQuery, ParsingContext parsingContext)
        {
            if (lookQuery.ExamineQuery == null) return;

            var luceneSearchCriteria = lookQuery.ExamineQuery as LuceneSearchCriteria; // will be of type LookSearchCriteria when using the custom Look indexer/searcher

            if (luceneSearchCriteria != null && luceneSearchCriteria.Query != null)
            {
                parsingContext.QueryAdd(luceneSearchCriteria.Query, BooleanClause.Occur.MUST);
            }
        }
    }
}
