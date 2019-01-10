using Examine.LuceneEngine.Providers;
using Examine.LuceneEngine.SearchCriteria;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Spatial.Tier;
using Our.Umbraco.Look.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Perform a Look search
        /// </summary>
        /// <param name="lookQuery">A LookQuery model for the search criteria</param>
        /// <returns>A LookResult model for the search response</returns>
        public static LookResult RunQuery(LookQuery lookQuery)
        {
            // flag to indicate whether there are any query clauses in the supplied LookQuery
            bool hasQuery = lookQuery?.Compiled != null ? true : false;

            if (lookQuery == null)
            {
                return new LookResult("LookQuery object was null");
            }

            if (lookQuery.SearchingContext == null) // supplied by unit test to skip examine dependency
            {
                // attempt to get searching context from examine searcher name
                lookQuery.SearchingContext = LookService.GetSearchingContext(lookQuery.SearcherName);

                if (lookQuery.SearchingContext == null)
                {
                    return new LookResult("SearchingContext was null");
                }
            }

            if (lookQuery.Compiled == null)
            {
                BooleanQuery query = null; // the lucene query being built                                            
                Filter filter = null; // used for geospatial queries
                Sort sort = null;
                Func<string, IHtmlString> getHighlight = x => null;
                Func<int, double?> getDistance = x => null;

                query = new BooleanQuery();

                #region RawQuery

                if (!string.IsNullOrWhiteSpace(lookQuery.RawQuery))
                {
                    hasQuery = true;

                    query.Add(
                            new QueryParser(Lucene.Net.Util.Version.LUCENE_29, null, lookQuery.SearchingContext.Analyzer).Parse(lookQuery.RawQuery),
                            BooleanClause.Occur.MUST);
                }

                #endregion

                #region ExamineQuery

                if (lookQuery.ExamineQuery != null)
                {
                    var luceneSearchCriteria = lookQuery.ExamineQuery as LuceneSearchCriteria; // will be of type LookSearchCriteria when using the custom Look indexer/searcher

                    if (luceneSearchCriteria != null && luceneSearchCriteria.Query != null)
                    {
                        hasQuery = true;

                        query.Add(luceneSearchCriteria.Query, BooleanClause.Occur.MUST);
                    }
                }

                #endregion

                #region NodeQuery

                if (lookQuery.NodeQuery != null)
                {
                    hasQuery = true;

                    query.Add(new TermQuery(new Term(LookConstants.HasNodeField, "1")), BooleanClause.Occur.MUST);

                    if (lookQuery.NodeQuery.Types != null && lookQuery.NodeQuery.Types.Any())
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

                    switch (lookQuery.NodeQuery.DetachedQuery)
                    {
                        case DetachedQuery.ExcludeDetached:

                            query.Add(
                                    new TermQuery(new Term(LookConstants.IsDetachedField, "1")),
                                    BooleanClause.Occur.MUST_NOT);

                            break;

                        case DetachedQuery.OnlyDetached:

                            query.Add(
                                new TermQuery(new Term(LookConstants.IsDetachedField, "1")),
                                BooleanClause.Occur.MUST);

                            break;
                    }

                    if (lookQuery.NodeQuery.Cultures != null && lookQuery.NodeQuery.Cultures.Any())
                    {
                        var nodeCultureQuery = new BooleanQuery();

                        foreach(var nodeCulture in lookQuery.NodeQuery.Cultures)
                        {
                            nodeCultureQuery.Add(
                                new TermQuery(
                                    new Term(LookConstants.CultureField, nodeCulture.LCID.ToString())),
                                    BooleanClause.Occur.SHOULD);
                        }

                        query.Add(nodeCultureQuery, BooleanClause.Occur.MUST);
                    }

                    if (lookQuery.NodeQuery.Aliases != null && lookQuery.NodeQuery.Aliases.Any())
                    {
                        var nodeAliasQuery = new BooleanQuery();

                        foreach (var typeAlias in lookQuery.NodeQuery.Aliases)
                        {
                            nodeAliasQuery.Add(
                                                new TermQuery(new Term(LookConstants.NodeAliasField, typeAlias)),
                                                BooleanClause.Occur.SHOULD);
                        }

                        query.Add(nodeAliasQuery, BooleanClause.Occur.MUST);
                    }

                    if (lookQuery.NodeQuery.Keys != null && lookQuery.NodeQuery.Keys.Any())
                    {
                        if (lookQuery.NodeQuery.NotKeys != null)
                        {
                            var conflictKeys = lookQuery.NodeQuery.Keys.Where(x => lookQuery.NodeQuery.NotKeys.Contains(x));

                            if (conflictKeys.Any())
                            {
                                return new LookResult($"Conflict in NodeQuery, keys: '{ string.Join(",", conflictKeys) }' are in both Keys and NotKeys");
                            }
                        }

                        var keyQuery = new BooleanQuery();

                        foreach (var key in lookQuery.NodeQuery.Keys)
                        {
                            keyQuery.Add(
                                        new TermQuery(new Term(LookConstants.NodeKeyField, key)),
                                        BooleanClause.Occur.SHOULD);
                        }

                        query.Add(keyQuery, BooleanClause.Occur.MUST);
                    }

                    if (lookQuery.NodeQuery.NotIds != null && lookQuery.NodeQuery.NotIds.Any())
                    {
                        foreach (var exculudeId in lookQuery.NodeQuery.NotIds)
                        {
                            query.Add(
                                    new TermQuery(new Term(LookConstants.NodeIdField, exculudeId.ToString())),
                                    BooleanClause.Occur.MUST_NOT);
                        }
                    }

                    if (lookQuery.NodeQuery.NotKeys != null && lookQuery.NodeQuery.NotKeys.Any())
                    {
                        foreach (var excludeKey in lookQuery.NodeQuery.NotKeys)
                        {
                            query.Add(
                                    new TermQuery(new Term(LookConstants.NodeKeyField, excludeKey)),
                                    BooleanClause.Occur.MUST_NOT);
                        }
                    }
                }

                #endregion

                #region NameQuery

                if (lookQuery.NameQuery != null)
                {
                    hasQuery = true;

                    query.Add(new TermQuery(new Term(LookConstants.HasNameField, "1")), BooleanClause.Occur.MUST);

                    string wildcard1 = null;
                    string wildcard2 = null; // incase Contains specified with StartsWith and/or EndsWith

                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.StartsWith))
                    {
                        if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                        {
                            if (!lookQuery.NameQuery.Is.StartsWith(lookQuery.NameQuery.StartsWith))
                            {
                                return new LookResult("Conflict in NameQuery between Is and StartsWith");
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
                                return new LookResult("Conflict in NameQuery between Is and EndsWith");
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
                                return new LookResult("Conflict in NameQuery between Is and Contains");
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

                #endregion

                #region DateQuery

                if (lookQuery.DateQuery != null)
                {
                    hasQuery = true;

                    query.Add(new TermQuery(new Term(LookConstants.HasDateField, "1")), BooleanClause.Occur.MUST);

                    if (lookQuery.DateQuery.After.HasValue || lookQuery.DateQuery.Before.HasValue)
                    {
                        var includeLower = lookQuery.DateQuery.After == null || lookQuery.DateQuery.Boundary == DateBoundary.Inclusive || lookQuery.DateQuery.Boundary == DateBoundary.BeforeExclusiveAfterInclusive;
                        var includeUpper = lookQuery.DateQuery.Before == null || lookQuery.DateQuery.Boundary == DateBoundary.Inclusive || lookQuery.DateQuery.Boundary == DateBoundary.BeforeInclusiveAfterExclusive;

                        query.Add(
                                new TermRangeQuery(
                                        LookConstants.DateField,
                                        lookQuery.DateQuery.After.DateToLuceneString() ?? DateTime.MinValue.DateToLuceneString(),
                                        lookQuery.DateQuery.Before.DateToLuceneString() ?? DateTime.MaxValue.DateToLuceneString(),
                                        includeLower,
                                        includeUpper),
                                BooleanClause.Occur.MUST);
                    }
                }

                #endregion

                #region TextQuery

                if (lookQuery.TextQuery != null)
                {
                    hasQuery = true;

                    query.Add(new TermQuery(new Term(LookConstants.HasTextField, "1")), BooleanClause.Occur.MUST);

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
                                var queryScorer = new QueryScorer(searchTextQuery.Rewrite(lookQuery.SearchingContext.IndexSearcher.GetIndexReader()));

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

                #endregion

                #region TagQuery

                if (lookQuery.TagQuery != null)
                {
                    hasQuery = true;

                    query.Add(new TermQuery(new Term(LookConstants.HasTagsField, "1")), BooleanClause.Occur.MUST);

                    if (lookQuery.TagQuery.All != null && lookQuery.TagQuery.All.Any())
                    {
                        if (lookQuery.TagQuery.None != null)
                        {
                            var conflictTags = lookQuery.TagQuery.All.Where(x => lookQuery.TagQuery.None.Contains(x));

                            if (conflictTags.Any())
                            {
                                return new LookResult($"Conflict in TagQuery, tags: '{ string.Join(",", conflictTags) }' are in both AllTags and NotTags");
                            }
                        }

                        foreach (var tag in lookQuery.TagQuery.All)
                        {
                            query.Add(
                                    new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                    BooleanClause.Occur.MUST);
                        }
                    }

                    if (lookQuery.TagQuery.Any != null && lookQuery.TagQuery.Any.Any())
                    {
                        if (lookQuery.TagQuery.None != null)
                        {
                            var conflictTags = lookQuery.TagQuery.Any.Where(x => lookQuery.TagQuery.None.Contains(x));

                            if (conflictTags.Any())
                            {
                                return new LookResult($"Conflict in TagQuery, tags: '{ string.Join(",", conflictTags) }' are in both AnyTags and NotTags");
                            }
                        }

                        var anyTagQuery = new BooleanQuery();

                        foreach (var tag in lookQuery.TagQuery.Any)
                        {
                            anyTagQuery.Add(
                                            new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                            BooleanClause.Occur.SHOULD);
                        }

                        query.Add(anyTagQuery, BooleanClause.Occur.MUST);
                    }

                    if (lookQuery.TagQuery.None != null && lookQuery.TagQuery.None.Any())
                    {
                        foreach (var tag in lookQuery.TagQuery.None)
                        {
                            query.Add(
                                new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                BooleanClause.Occur.MUST_NOT);
                        }
                    }
                }

                #endregion

                #region LocationQuery
                
                if (lookQuery.LocationQuery != null)
                {
                    hasQuery = true;

                    query.Add(new TermQuery(new Term(LookConstants.HasLocationField, "1")), BooleanClause.Occur.MUST);

                    if (lookQuery.LocationQuery.Location != null)
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
                }

                #endregion

                if (hasQuery)
                {
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

                        // SortOn.Distance already set (if valid)
                    }

                    lookQuery.Compiled = new LookQueryCompiled(
                                                        lookQuery,
                                                        query,
                                                        filter,
                                                        sort ?? new Sort(SortField.FIELD_SCORE),
                                                        getHighlight,
                                                        getDistance);
                }
            }

            if (!hasQuery)
            {
                return new LookResult("No query clauses supplied"); // empty failure
            }

            TopDocs topDocs = lookQuery
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
                                LookService.GetLookMatches(
                                                        lookQuery.SearchingContext.IndexSearcher,
                                                        topDocs,
                                                        lookQuery.RequestFields ?? LookService.Instance.RequestFields,
                                                        lookQuery.Compiled.GetHighlight,
                                                        lookQuery.Compiled.GetDistance),
                                topDocs.TotalHits,
                                facets != null ? facets.ToArray() : new Facet[] { });
            }

            return new LookResult(); // empty success
        }
    }
}
