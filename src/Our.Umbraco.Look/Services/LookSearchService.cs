using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Spatial.Tier;
using Lucene.Net.Spatial.Tier.Projectors;
using Our.Umbraco.Look.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core.Logging;
using UmbracoExamine;

namespace Our.Umbraco.Look.Services
{
    public static class LookSearchService
    {
        /// <summary>
        ///  Main searching method
        /// </summary>
        /// <param name="lookQuery"></param>
        /// <returns>an IEnumerableWithTotal</returns>
        public static LookResult Query(LookQuery lookQuery)
        {
            if (lookQuery == null)
            {
                LogHelper.Debug(typeof(LookService), "The supplied LookQuery was null");

                return LookResult.Empty;
            }

            // the lucene query being built
            var query = new BooleanQuery(); 

            // pasre the supplied lookQuery
            if (!string.IsNullOrWhiteSpace(lookQuery.RawQuery))
            {
                query.Add(
                        new QueryParser(Lucene.Net.Util.Version.LUCENE_29, null, LookService.Analyzer).Parse(lookQuery.RawQuery),
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
                                            new TermQuery(new Term(UmbracoContentIndexer.NodeTypeAliasFieldName, typeAlias)),
                                            BooleanClause.Occur.SHOULD);
                    }

                    query.Add(typeAliasQuery, BooleanClause.Occur.MUST);
                }

                if (lookQuery.NodeQuery.ExcludeIds != null) // TODO: rename to NotIds (like tags)
                {
                    foreach(var exculudeId in lookQuery.NodeQuery.ExcludeIds)
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
                                GetDate(lookQuery.DateQuery.After) ?? GetDate(DateTime.MinValue),
                                GetDate(lookQuery.DateQuery.Before) ?? GetDate(DateTime.MaxValue),
                                true,
                                true),
                        BooleanClause.Occur.MUST);
            }

            if (lookQuery.TextQuery != null)
            {
                if (!string.IsNullOrWhiteSpace(lookQuery.TextQuery.SearchText))
                {
                    // TODO: wildcards

                    if (lookQuery.TextQuery.Fuzzyness > 0)
                    {
                        query.Add(
                                new FuzzyQuery(
                                    new Term(LookConstants.TextField, lookQuery.TextQuery.SearchText), 
                                    lookQuery.TextQuery.Fuzzyness),
                                BooleanClause.Occur.MUST);
                    }
                    else
                    {
                        query.Add(
                                new QueryParser(Lucene.Net.Util.Version.LUCENE_29, LookConstants.TextField, LookService.Analyzer).Parse(lookQuery.TextQuery.SearchText),
                                BooleanClause.Occur.MUST);
                    }
                }
            }

            if (lookQuery.TagQuery != null)
            {
                if (lookQuery.TagQuery.AllTags != null)
                {
                    foreach(var tag in lookQuery.TagQuery.AllTags)
                    {
                        query.Add(
                                new TermQuery(new Term(LookConstants.TagsField, tag)),
                                BooleanClause.Occur.MUST);
                    }
                }

                if (lookQuery.TagQuery.AnyTags != null)
                {                    
                    var anyTagQuery = new BooleanQuery();

                    foreach(var tag in lookQuery.TagQuery.AnyTags)
                    {
                        anyTagQuery.Add(
                                        new TermQuery(new Term(LookConstants.TagsField, tag)),
                                        BooleanClause.Occur.SHOULD);
                    }
                    
                    query.Add(anyTagQuery, BooleanClause.Occur.MUST);
                }

                // TODO: NotTags
            }

            // optional filter used for geospatial queries
            Filter filter = null;
            Sort sort = null;

            // optional actions to execute when peforming a query
            Func<int, double?> getDistance = x => null;

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
                                            CartesianTierPlotter.DefaltFieldPrefix,
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

            // TODO: allow directory to be injected in for testing outside of umbraco
            var indexSearcher = new IndexSearcher(((LuceneIndexer)LookService.Indexer).GetLuceneDirectory(), true); 

            // do the Lucene search
            var topDocs = indexSearcher.Search(
                                            query,
                                            filter,
                                            LookService.MaxLuceneResults,
                                            sort ?? new Sort(SortField.FIELD_SCORE));

            if (topDocs.TotalHits > 0)
            {
                Facet[] facets = null;

                // facets
                if (lookQuery.TagQuery != null && lookQuery.TagQuery.GetFacets)
                {
                    var simpleFacetedSearch = new SimpleFacetedSearch(indexSearcher.GetIndexReader(), LookConstants.TagsField);

                    Query facetQuery = null;

                    if (filter != null)
                    {
                        facetQuery = new FilteredQuery(query, filter);
                    }
                    else
                    {
                        facetQuery = query;
                    }

                    var facetResult = simpleFacetedSearch.Search(facetQuery);

                    facets = facetResult
                                .HitsPerFacet
                                .Select(
                                    x => new Facet()
                                    {
                                        Tag = x.Name.ToString(),
                                        Count = Convert.ToInt32(x.HitCount)
                                    }
                                )
                                .ToArray();
                }

                Func<string, IHtmlString> getHighlight = null;

                // setup the getHightlight func if required
                if (lookQuery.TextQuery != null && lookQuery.TextQuery.GetHighlight && !string.IsNullOrWhiteSpace(lookQuery.TextQuery.SearchText))
                {
                    var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, LookConstants.TextField, LookService.Analyzer);

                    var queryScorer = new QueryScorer(queryParser
                                                        .Parse(lookQuery.TextQuery.SearchText)
                                                        .Rewrite(indexSearcher.GetIndexReader()));

                    var highlighter = new Highlighter(new SimpleHTMLFormatter("<strong>", "</strong>"), queryScorer);

                    // update the getHightlight func
                    getHighlight = (x) =>
                    {
                        var tokenStream = LookService.Analyzer.TokenStream(LookConstants.TextField, new StringReader(x));

                        var highlight = highlighter.GetBestFragments(
                                                        tokenStream,
                                                        x,
                                                        1, // max number of fragments
                                                        "...");

                        return new HtmlString(highlight);
                    };
                }

                return new LookResult(LookSearchService.GetLookMatches(
                                                            lookQuery,
                                                            indexSearcher,
                                                            topDocs,
                                                            getHighlight,
                                                            getDistance),
                                        topDocs.TotalHits,
                                        facets);
            }

            return LookResult.Empty;
        }

        /// <summary>
        /// Supplied with the result of a Lucene query, this method will yield a constructed LookMatch for each in order
        /// </summary>
        /// <param name="indexSearcher">The searcher supplied to get the Lucene doc for each id in the Lucene results (topDocs)</param>
        /// <param name="topDocs">The results of the Lucene query (a collection of ids in an order)</param>
        /// <param name="getHighlight">Function used to get the highlight text for a given result text</param>
        /// <param name="getDistance">Function used to calculate distance (if a location was supplied in the original query)</param>
        /// <returns></returns>
        private static IEnumerable<LookMatch> GetLookMatches(
                                                    LookQuery lookQuery,
                                                    IndexSearcher indexSearcher,
                                                    TopDocs topDocs,
                                                    Func<string, IHtmlString> getHighlight,
                                                    Func<int, double?> getDistance)
        {
            // flag to indicate that the query has requested the full text to be returned
            bool getText = lookQuery.TextQuery != null && lookQuery.TextQuery.GetText;

            var fields = new List<string>();

            fields.Add(LuceneIndexer.IndexNodeIdFieldName); // "__NodeId"
            fields.Add(LookConstants.NameField);
            fields.Add(LookConstants.DateField);

            // if a highlight function is supplied (or text requested)
            if (getHighlight != null || getText)  { fields.Add(LookConstants.TextField); }

            fields.Add(LookConstants.TagsField);
            fields.Add(LookConstants.LocationField);

            var mapFieldSelector = new MapFieldSelector(fields.ToArray());

            // if highlight func does not exist, then create one to always return null
            if (getHighlight == null) { getHighlight = x => null; }

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var docId = scoreDoc.doc;

                var doc = indexSearcher.Doc(docId, mapFieldSelector);

                var lookMatch = new LookMatch(
                    Convert.ToInt32(doc.Get(LuceneIndexer.IndexNodeIdFieldName)),
                    getHighlight(doc.Get(LookConstants.TextField)),
                    getText ? doc.Get(LookConstants.TextField) : null,
                    LookSearchService.GetTags(doc.GetFields(LookConstants.TagsField)),
                    LookSearchService.GetDate(doc.Get(LookConstants.DateField)),
                    doc.Get(LookConstants.NameField),
                    doc.Get(LookConstants.LocationField) != null ? new Location(doc.Get(LookConstants.LocationField)) : null,
                    getDistance(docId),
                    scoreDoc.score
                );

                yield return lookMatch;
            }
        }

        /// <summary>
        /// Helper for when building a look match obj
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string[] GetTags(Field[] fields)
        {
            if (fields != null)
            {
                return fields
                        .Select(y => y.StringValue())
                        .Where(y => !string.IsNullOrWhiteSpace(y))
                        .ToArray();
            }

            return new string[] { };
        }

        /// <summary>
        /// Helper for when building a look match obj
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        private static DateTime? GetDate(string dateValue)
        {
            DateTime? date = null;

            try
            {
                date = DateTools.StringToDate(dateValue);
            }
            catch
            {
                LogHelper.Info(typeof(LookSearchService), $"Unable to convert string '{dateValue}' into a DateTime");
            }

            return date;
        }

        /// <summary>
        /// the reverse - get string from DateTime?
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string GetDate(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                return DateTools.DateToString(dateTime.Value, DateTools.Resolution.SECOND);
            }

            return null;
        }
    }
}
