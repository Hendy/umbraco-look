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
        /// Perform a Look search
        /// </summary>
        /// <param name="lookQuery">A LookQuery model for the search criteria</param>
        /// <returns>A LookResult model for the search response</returns>
        public static LookResult Query(LookQuery lookQuery)
        {
            if (lookQuery == null)
            {
                return new LookResult("Unable to perform query, as supplied LookQuery object was null");
            }

            if (lookQuery.SearchingContext == null) // if not supplied by a unit test
            {
                // attempt to get searching context from examine searcher name
                lookQuery.SearchingContext = LookService.GetSearchingContext(lookQuery.SearcherName);

                if (lookQuery.SearchingContext == null)
                {
                    return new LookResult("Unable to perform query, as searchingContext was null");
                }
            }

            if (lookQuery.Compiled == null)
            {
                BooleanQuery query = null; // the lucene query being built                                            
                Filter filter = null; // used for geospatial queries
                Sort sort = null;
                Func<string, IHtmlString> getHighlight = null;
                Func<int, double?> getDistance = x => null;

                query = new BooleanQuery();

                if (!string.IsNullOrWhiteSpace(lookQuery.RawQuery))
                {
                    query.Add(
                            new QueryParser(Lucene.Net.Util.Version.LUCENE_29, null, lookQuery.SearchingContext.Analyzer).Parse(lookQuery.RawQuery),
                            BooleanClause.Occur.MUST);
                }

                if (lookQuery.NodeQuery != null)
                {
                    if (lookQuery.NodeQuery.Types != null)
                    {
                        var nodeTypeQuery = new BooleanQuery();

                        foreach(var nodeType in lookQuery.NodeQuery.Types)
                        {
                            nodeTypeQuery.Add(
                                new TermQuery(
                                    new Term(LookConstants.NodeTypeField, nodeType.ToString())),
                                    BooleanClause.Occur.SHOULD);
                        }

                        query.Add(nodeTypeQuery, BooleanClause.Occur.MUST);
                    }

                    if (lookQuery.NodeQuery.Aliases != null)
                    {
                        var typeAliasQuery = new BooleanQuery();

                        foreach (var typeAlias in lookQuery.NodeQuery.Aliases)
                        {
                            typeAliasQuery.Add(
                                                new TermQuery(new Term(UmbracoContentIndexer.NodeTypeAliasFieldName, typeAlias.ToLower())), // TODO: store alias in a custom field to keep casing ?
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
                                    BooleanClause.Occur.MUST_NOT);
                        }
                    }
                }

                if (lookQuery.NameQuery != null)
                {
                    string wildcard1 = null;
                    string wildcard2 = null; // incase Contains specified with StartsWith and/or EndsWith

                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.StartsWith))
                    {
                        if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                        {
                            if (!lookQuery.NameQuery.Is.StartsWith(lookQuery.NameQuery.StartsWith))
                            {
                                return new LookResult("Conlict in NameQuery");
                            }
                        }
                        else
                        {
                            wildcard1 = lookQuery.NameQuery.StartsWith + "*";
                        }
                    }

                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.EndsWith))
                    {
                        if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))                            
                        {
                            if (!lookQuery.NameQuery.Is.EndsWith(lookQuery.NameQuery.EndsWith))
                            {
                                return new LookResult("Conlict in NameQuery");
                            }                            
                        }
                        else
                        {
                            if (wildcard1 == null)
                            {
                                wildcard1 = "*" + lookQuery.NameQuery.EndsWith;
                            }
                            else
                            {
                                wildcard1 += lookQuery.NameQuery.EndsWith;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.Contains))
                    {
                        if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                        {
                            if (!lookQuery.NameQuery.Is.Contains(lookQuery.NameQuery.Contains))
                            {
                                return new LookResult("Conlict in NameQuery");
                            }
                        }
                        else
                        {
                            if (wildcard1 == null)
                            {
                                wildcard1 = "*" + lookQuery.NameQuery.Contains + "*";
                            }
                            else
                            {
                                wildcard2 = "*" + lookQuery.NameQuery.Contains + "*";
                            }
                        }
                    }

                    var nameField = lookQuery.NameQuery.CaseSensitive ? LookConstants.NameField : LookConstants.NameField + "_Lowered";

                    if (wildcard1 != null)
                    {                        
                        var wildcard = lookQuery.NameQuery.CaseSensitive ? wildcard1 : wildcard1.ToLower();

                        query.Add(new WildcardQuery(new Term(nameField, wildcard)), BooleanClause.Occur.MUST);

                        if (wildcard2 != null)
                        {
                            wildcard = lookQuery.NameQuery.CaseSensitive ? wildcard2 : wildcard2.ToLower();

                            query.Add(new WildcardQuery(new Term(nameField, wildcard)), BooleanClause.Occur.MUST);
                        }
                    }

                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                    {
                        var isText = lookQuery.NameQuery.CaseSensitive ? lookQuery.NameQuery.Is : lookQuery.NameQuery.Is.ToLower();

                        query.Add(new TermQuery(new Term(nameField, isText)), BooleanClause.Occur.MUST);
                    }
                }

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
                        var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, LookConstants.TextField, lookQuery.SearchingContext.Analyzer);

                        Query searchTextQuery = null;

                        try
                        {
                            searchTextQuery = queryParser.Parse(lookQuery.TextQuery.SearchText);
                        }
                        catch
                        {
                            return new LookResult($"Unable to parse LookQuery.TextQuery.SearchText: '{ lookQuery.TextQuery.SearchText }' into a Lucene query");
                        }

                        if (searchTextQuery != null)
                        {
                            query.Add(searchTextQuery, BooleanClause.Occur.MUST);

                            if (lookQuery.TextQuery.GetHighlight)
                            {
                                var queryScorer = new QueryScorer(queryParser
                                                                    .Parse(lookQuery.TextQuery.SearchText)
                                                                    .Rewrite(lookQuery.SearchingContext.IndexSearcher.GetIndexReader()));

                                var highlighter = new Highlighter(new SimpleHTMLFormatter("<strong>", "</strong>"), queryScorer);

                                getHighlight = (x) =>
                                {
                                    var tokenStream = lookQuery.SearchingContext.Analyzer.TokenStream(LookConstants.TextField, new StringReader(x));

                                    var highlight = highlighter.GetBestFragments(
                                                                    tokenStream,
                                                                    x,
                                                                    1, // max number of fragments
                                                                    "...");

                                    return new HtmlString(highlight);
                                };
                            }
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

                    filter = distanceQueryBuilder.Filter;

                    if (lookQuery.SortOn == SortOn.Distance)
                    {
                        sort = new Sort(
                                    new SortField(
                                        LookConstants.DistanceField,
                                        new DistanceFieldComparatorSource(distanceQueryBuilder.DistanceFilter)));
                    }

                    getDistance = new Func<int, double?>(x =>
                    {
                        if (distanceQueryBuilder.DistanceFilter.Distances.ContainsKey(x))
                        {
                            return distanceQueryBuilder.DistanceFilter.Distances[x];
                        }

                        return null;
                    });
                }

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

                    //case SortOn.Distance: // already set if valid
                    //    break;
                }
                
                lookQuery.Compiled = new LookQueryCompiled(
                                                    lookQuery, 
                                                    query, 
                                                    filter, 
                                                    sort ?? new Sort(SortField.FIELD_SCORE), 
                                                    getHighlight, 
                                                    getDistance);
            }

            // look query compiled, so do the Lucene search
            var topDocs = lookQuery
                            .SearchingContext
                            .IndexSearcher
                            .Search(
                                lookQuery.Compiled.Query,
                                lookQuery.Compiled.Filter,
                                LookService.MaxLuceneResults,
                                lookQuery.Compiled.Sort);

            if (topDocs.TotalHits > 0)
            {
                List<Facet> facets = null;

                if (lookQuery.TagQuery != null && lookQuery.TagQuery.GetFacets != null)
                {
                    facets = new List<Facet>();

                    Query facetQuery = lookQuery.Compiled.Filter != null 
                                            ? (Query)new FilteredQuery(lookQuery.Compiled.Query, lookQuery.Compiled.Filter) 
                                            : lookQuery.Compiled.Query;

                    // do a facet query for each group in the array
                    foreach (var group in lookQuery.TagQuery.GetFacets)
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
                                            Tag = new LookTag(group, x.Name.ToString()),
                                            Count = Convert.ToInt32(x.HitCount)
                                        }
                                    ));
                    }
                }

                return new LookResult(LookService.GetLookMatches(
                                                        lookQuery.SearchingContext.IndexSearcher,
                                                        topDocs,
                                                        lookQuery.Compiled.GetHighlight,
                                                        lookQuery.TextQuery != null && lookQuery.TextQuery.GetText,
                                                        lookQuery.Compiled.GetDistance),
                                        topDocs.TotalHits,
                                        facets != null ? facets.ToArray() : new Facet[] { },
                                        lookQuery);
            }

            return new LookResult(lookQuery);
        }
    }
}
