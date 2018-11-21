using Examine.LuceneEngine.Providers;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Spatial.Tier;
using Our.Umbraco.Look.Extensions;
using Our.Umbraco.Look.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using UmbracoExamine;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        ///  Main searching method
        /// </summary>
        /// <param name="lookQuery"></param>
        /// <returns>an IEnumerableWithTotal</returns>
        public static LookResult Query(LookQuery lookQuery)
        {
            SearchingContext searchingContext = null;
            BooleanQuery query = null; // the lucene query being built                                            
            Filter filter = null; // used for geospatial queries
            Sort sort = null;
            Func<int, double?> getDistance = x => null;
            Func<string, IHtmlString> getHighlight = null;

            if (lookQuery == null)
            {
                return new LookResult("Unable to perform query, as supplied LookQuery object was null");
            }

            searchingContext = LookService.GetSearchingContext(lookQuery.SearcherName);

            if (searchingContext == null)
            {
                return new LookResult("Unable to perform query, as Examine searcher not found");
            }

            query = new BooleanQuery();

            if (!string.IsNullOrWhiteSpace(lookQuery.RawQuery))
            {
                query.Add(
                        new QueryParser(Lucene.Net.Util.Version.LUCENE_29, null, searchingContext.Analyzer).Parse(lookQuery.RawQuery),
                        BooleanClause.Occur.MUST);
            }

            if (lookQuery.NodeQuery != null)
            {
                if (lookQuery.NodeQuery.TypeAliases != null)
                {
                    var typeAliasQuery = new BooleanQuery();

                    foreach (var typeAlias in lookQuery.NodeQuery.TypeAliases)
                    {
                        typeAliasQuery.Add(
                                            new TermQuery(new Term(UmbracoContentIndexer.NodeTypeAliasFieldName, typeAlias.ToLower())), // TODO: store alias in a custom field to keep casing
                                            BooleanClause.Occur.SHOULD);
                    }

                    query.Add(typeAliasQuery, BooleanClause.Occur.MUST);
                }

                if (lookQuery.NodeQuery.NotIds != null)
                {
                    foreach (var exculudeId in lookQuery.NodeQuery.NotIds)
                    {
                        query.Add(
                                new TermQuery(new Term(UmbracoContentIndexer.IndexNodeIdFieldName, exculudeId.ToString())),
                                BooleanClause.Occur.MUST);
                    }
                }
            }

            //if (lookQuery.NameQuery != null)
            //{
            //}

            if (lookQuery.DateQuery != null && (lookQuery.DateQuery.After.HasValue || lookQuery.DateQuery.Before.HasValue))
            {
                query.Add(
                        new TermRangeQuery(
                                LookConstants.DateField,
                                lookQuery.DateQuery.After.DateToLuceneString() ?? DateTime.MinValue.DateToLuceneString(),
                                lookQuery.DateQuery.Before.DateToLuceneString() ?? DateTime.MaxValue.DateToLuceneString(),
                                true,
                                true),
                        BooleanClause.Occur.MUST);
            }

            if (lookQuery.TextQuery != null)
            {
                if (!string.IsNullOrWhiteSpace(lookQuery.TextQuery.SearchText))
                {
                    //if (lookQuery.TextQuery.Fuzzyness > 0)
                    //{
                    //    query.Add(
                    //            new FuzzyQuery(
                    //                new Term(LookConstants.TextField, lookQuery.TextQuery.SearchText),
                    //                lookQuery.TextQuery.Fuzzyness),
                    //            BooleanClause.Occur.MUST);
                    //}

                    Query searchTextQuery = null;

                    try
                    { 
                        searchTextQuery = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, LookConstants.TextField, searchingContext.Analyzer)
                                                .Parse(lookQuery.TextQuery.SearchText);
                    }
                    catch
                    {
                        return new LookResult($"Unable to parse LookQuery.TextQuery.SearchText: '{ lookQuery.TextQuery.SearchText }' into a Lucene query");
                    }

                    if (searchTextQuery != null)
                    {
                        query.Add(searchTextQuery, BooleanClause.Occur.MUST);
                    }
                }
            }

            if (lookQuery.TagQuery != null)
            {
                if (lookQuery.TagQuery.AllTags != null)
                {
                    if (lookQuery.TagQuery.NotTags != null)
                    {
                        var conflictTags = lookQuery.TagQuery.AllTags.Where(x => !lookQuery.TagQuery.NotTags.Contains(x));

                        if (conflictTags.Any())
                        {
                            return new LookResult($"Query conflict, tags: '{ string.Join(",", conflictTags) }' are in both AllTags and NotTags");
                        }
                    }

                    foreach (var tag in lookQuery.TagQuery.AllTags)
                    {
                        query.Add(
                                new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                BooleanClause.Occur.MUST);
                    }
                }

                if (lookQuery.TagQuery.AnyTags != null)
                {
                    if (lookQuery.TagQuery.NotTags != null)
                    {
                        var conflictTags = lookQuery.TagQuery.AnyTags.Where(x => !lookQuery.TagQuery.NotTags.Contains(x));

                        if (conflictTags.Any())
                        {
                            return new LookResult($"Query conflict, tags: '{ string.Join(",", conflictTags) }' are in both AnyTags and NotTags");
                        }
                    }

                    var anyTagQuery = new BooleanQuery();

                    foreach (var tag in lookQuery.TagQuery.AnyTags)
                    {
                        anyTagQuery.Add(
                                        new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                        BooleanClause.Occur.SHOULD);
                    }

                    query.Add(anyTagQuery, BooleanClause.Occur.MUST);
                }

                if (lookQuery.TagQuery.NotTags != null)
                {
                    foreach (var tag in lookQuery.TagQuery.NotTags)
                    {
                        query.Add(
                            new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                            BooleanClause.Occur.MUST_NOT);
                    }
                }
            }

            // Location
            if (lookQuery.LocationQuery != null && lookQuery.LocationQuery.Location != null)
            {
                double maxDistance = LookService.MaxDistance;

                if (lookQuery.LocationQuery.MaxDistance != null)
                {
                    maxDistance = Math.Min(lookQuery.LocationQuery.MaxDistance.GetMiles(), maxDistance);
                }

                var distanceQueryBuilder = new DistanceQueryBuilder(
                                            lookQuery.LocationQuery.Location.Latitude,
                                            lookQuery.LocationQuery.Location.Longitude,
                                            maxDistance,
                                            LookConstants.LocationField + "_Latitude",
                                            LookConstants.LocationField + "_Longitude",
                                            LookConstants.LocationTierFieldPrefix,
                                            true);

                // update filter
                filter = distanceQueryBuilder.Filter;

                if (lookQuery.SortOn == SortOn.Distance)
                {
                    // update sort
                    sort = new Sort(
                                new SortField(
                                    LookConstants.DistanceField,
                                    new DistanceFieldComparatorSource(distanceQueryBuilder.DistanceFilter)));
                }

                // update getDistance func
                getDistance = new Func<int, double?>(x =>
                {
                    if (distanceQueryBuilder.DistanceFilter.Distances.ContainsKey(x))
                    {
                        return distanceQueryBuilder.DistanceFilter.Distances[x];
                    }

                    return null;
                });
            }

            // sorting (distance sort may already have been set)
            switch (lookQuery.SortOn)
            {
                case SortOn.Name: // a -> z
                    sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.NameField, SortField.STRING));
                    break;

                case SortOn.DateAscending: // oldest -> newest
                    sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField, SortField.LONG, false));
                    break;

                case SortOn.DateDescending: // newest -> oldest
                    sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField, SortField.LONG, true));
                    break;
            }

            // do the Lucene search
            var topDocs = searchingContext
                                .IndexSearcher
                                .Search(
                                    query,
                                    filter,
                                    LookService.MaxLuceneResults,
                                    sort ?? new Sort(SortField.FIELD_SCORE));

            if (topDocs.TotalHits > 0)
            {
                List<Facet> facets = null;

                if (lookQuery.TagQuery != null && lookQuery.TagQuery.GetFacets != null)
                {
                    facets = new List<Facet>();

                    Query facetQuery = filter != null ? (Query)new FilteredQuery(query, filter) : query;

                    // do a facet query for each group in the array
                    foreach (var group in lookQuery.TagQuery.GetFacets)
                    {
                        var simpleFacetedSearch = new SimpleFacetedSearch(
                                                        searchingContext.IndexSearcher.GetIndexReader(),
                                                        LookConstants.TagsField + group);

                        var facetResult = simpleFacetedSearch.Search(facetQuery);

                        facets.AddRange(
                                facetResult
                                    .HitsPerFacet
                                    .Select(
                                        x => new Facet()
                                        {
                                            Tag = new Tag(group, x.Name.ToString()),
                                            Count = Convert.ToInt32(x.HitCount)
                                        }
                                    ));
                    }
                }

                // setup the getHightlight func if required
                if (lookQuery.TextQuery != null && !string.IsNullOrWhiteSpace(lookQuery.TextQuery.SearchText) && lookQuery.TextQuery.GetHighlight)
                {
                    var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, LookConstants.TextField, searchingContext.Analyzer);

                    var queryScorer = new QueryScorer(queryParser
                                                        .Parse(lookQuery.TextQuery.SearchText)
                                                        .Rewrite(searchingContext.IndexSearcher.GetIndexReader()));

                    var highlighter = new Highlighter(new SimpleHTMLFormatter("<strong>", "</strong>"), queryScorer);

                    // update the getHightlight func
                    getHighlight = (x) =>
                    {
                        var tokenStream = searchingContext.Analyzer.TokenStream(LookConstants.TextField, new StringReader(x));

                        var highlight = highlighter.GetBestFragments(
                                                        tokenStream,
                                                        x,
                                                        1, // max number of fragments
                                                        "...");

                        return new HtmlString(highlight);
                    };
                }

                return new LookResult(LookService.GetLookMatches(
                                                        lookQuery,
                                                        searchingContext.IndexSearcher,
                                                        topDocs,
                                                        getHighlight,
                                                        getDistance),
                                        topDocs.TotalHits,
                                        facets != null ? facets.ToArray() : new Facet[] { });
            }

            return new LookResult();
        }
    }
}
