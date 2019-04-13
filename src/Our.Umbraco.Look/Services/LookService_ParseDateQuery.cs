using Examine.LuceneEngine.Providers;
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
        private static void ParseDateQuery(ParsingContext parsingContext, LookQuery lookQuery)
        {
            // handle sorting first, as date query clause not required for a date sort
            switch (lookQuery.SortOn)
            {
                case SortOn.DateAscending: // oldest -> newest
                    parsingContext.Sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField, SortField.LONG, false));
                    break;

                case SortOn.DateDescending: // newest -> oldest
                    parsingContext.Sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField, SortField.LONG, true));
                    break;
            }

            if (lookQuery.DateQuery == null) return;

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
