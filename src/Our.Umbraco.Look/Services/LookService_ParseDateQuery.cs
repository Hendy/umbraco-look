using Lucene.Net.Index;
using Lucene.Net.Search;
using Our.Umbraco.Look.Extensions;
using Our.Umbraco.Look.Models;
using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookQuery">The query to parse</param>
        /// <param name="parsingContext"></param>
        private static void ParseDateQuery(LookQuery lookQuery, ParsingContext parsingContext)
        {
            if (lookQuery.DateQuery != null)
            {
                parsingContext.QueryAdd(new TermQuery(new Term(LookConstants.HasDateField, "1")), BooleanClause.Occur.MUST);

                if (lookQuery.DateQuery.After.HasValue || lookQuery.DateQuery.Before.HasValue)
                {
                    var includeLower = lookQuery.DateQuery.After == null || lookQuery.DateQuery.Boundary == DateBoundary.Inclusive || lookQuery.DateQuery.Boundary == DateBoundary.BeforeExclusiveAfterInclusive;
                    var includeUpper = lookQuery.DateQuery.Before == null || lookQuery.DateQuery.Boundary == DateBoundary.Inclusive || lookQuery.DateQuery.Boundary == DateBoundary.BeforeInclusiveAfterExclusive;

                    parsingContext.QueryAdd(
                            new TermRangeQuery(
                                    LookConstants.DateField,
                                    lookQuery.DateQuery.After.DateToLuceneString() ?? DateTime.MinValue.DateToLuceneString(),
                                    lookQuery.DateQuery.Before.DateToLuceneString() ?? DateTime.MaxValue.DateToLuceneString(),
                                    includeLower,
                                    includeUpper),
                            BooleanClause.Occur.MUST);
                }
            }
        }
    }
}
