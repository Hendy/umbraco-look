using Lucene.Net.Search;
using Our.Umbraco.Look.Exceptions;
using Our.Umbraco.Look.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Perform a Look search
        /// </summary>
        /// <param name="lookQuery">A LookQuery model for the search criteria</param>
        /// <returns>A LookResult model for the search response</returns>
        internal static LookResult Search(LookQuery lookQuery)
        {
            if (lookQuery == null)
            {
                return LookResult.Error("LookQuery object was null");
            }

            if (lookQuery.SearchingContext == null) // supplied by unit test to skip examine dependency
            {
                // attempt to get searching context from examine searcher name
                lookQuery.SearchingContext = LookService.GetSearchingContext(lookQuery.SearcherName);

                if (lookQuery.SearchingContext == null)
                {
                    return LookResult.Error("SearchingContext was null");
                }
            }

            if (lookQuery.Compiled == null)
            {
                var parsingContext = new ParsingContext(); // for building/compiling the query

                try
                {
                    LookService.ParseRawQuery(parsingContext, lookQuery);

                    LookService.ParseExamineQuery(parsingContext, lookQuery);

                    LookService.ParseNodeQuery(parsingContext, lookQuery);

                    LookService.ParseNameQuery(parsingContext, lookQuery);

                    LookService.ParseDateQuery(parsingContext, lookQuery);

                    LookService.ParseTextQuery(parsingContext, lookQuery);

                    LookService.ParseTagQuery(parsingContext, lookQuery);

                    LookService.ParseLocationQuery(parsingContext, lookQuery);
                }
                catch (ParsingException parsingException)
                {
                    return LookResult.Error(parsingException.Message);
                }

                if (parsingContext.HasQuery)
                {
                    lookQuery.Compiled = new LookQueryCompiled(lookQuery, parsingContext);
                }
                else
                {
                    return LookResult.Error("Unable to compile query - a query clause is required"); // empty failure
                }
            }

            TopDocs topDocs = lookQuery
                                .SearchingContext
                                .IndexSearcher
                                .Search(
                                    lookQuery.Compiled.Query,
                                    lookQuery.Compiled.Filter,
                                    LookService._maxLuceneResults,
                                    lookQuery.Compiled.Sort);

            if (topDocs.TotalHits > 0)
            {
                List<Facet> facets = null;

                if (lookQuery.TagQuery != null && lookQuery.TagQuery.FacetOn != null)
                {
                    facets = new List<Facet>();

                    Query facetQuery = lookQuery.Compiled.Filter != null 
                                            ? (Query)new FilteredQuery(lookQuery.Compiled.Query, lookQuery.Compiled.Filter) 
                                            : lookQuery.Compiled.Query;

                    // do a facet query for each group in the array
                    foreach (var group in lookQuery.TagQuery.FacetOn.TagGroups)
                    {
                        var simpleFacetedSearch = new SimpleFacetedSearch(
                                                        lookQuery.SearchingContext.IndexSearcher.GetIndexReader(),
                                                        LookConstants.TagsField + group);

                        var facetResult = simpleFacetedSearch.Search(facetQuery);

                        facets.AddRange(
                                facetResult
                                    .HitsPerFacet
                                    .Select(
                                        x => new Facet()
                                        {
                                            Tags = new LookTag[] { new LookTag(group, x.Name.ToString()) },
                                            Count = Convert.ToInt32(x.HitCount)
                                        }
                                    ));
                    }
                }

                return new LookResult(
                                lookQuery,
                                topDocs,
                                facets != null ? facets.ToArray() : new Facet[] { });
            }

            return LookResult.Empty(); // empty success
        }
    }
}
