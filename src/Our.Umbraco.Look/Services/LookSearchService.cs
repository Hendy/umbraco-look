using Examine.LuceneEngine.Providers;
using Examine.LuceneEngine.SearchCriteria;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Spatial.Tier;
using Lucene.Net.Spatial.Tier.Projectors;
using Our.Umbraco.Look.Interfaces;
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
        public static IEnumerableWithTotal<LookMatch> Query(LookQuery lookQuery)
        {
            if (lookQuery == null)
            {
                LogHelper.Warn(typeof(LookService), "Supplied search query was null");

                return new EnumerableWithTotal<LookMatch>(Enumerable.Empty<LookMatch>(), 0);
            }

            var searchProvider = LookService.Searcher;

            var searchCriteria = searchProvider.CreateSearchCriteria();

            var query = searchCriteria.Field(string.Empty, string.Empty);

            // Text
            if (lookQuery.TextQuery != null)
            {
                if (!string.IsNullOrWhiteSpace(lookQuery.TextQuery.SearchText))
                {
                    if (lookQuery.TextQuery.Fuzzyness > 0)
                    {
                        query.And().Field(LookConstants.TextField, lookQuery.TextQuery.SearchText.Fuzzy(lookQuery.TextQuery.Fuzzyness));
                    }
                    else
                    {
                        query.And().Field(LookConstants.TextField, lookQuery.TextQuery.SearchText);
                    }
                }
            }

            // Tags
            if (lookQuery.TagQuery != null)
            {
                var allTags = new List<string>();
                var anyTags = new List<string>();

                if (lookQuery.TagQuery.AllTags != null)
                {
                    allTags.AddRange(lookQuery.TagQuery.AllTags);
                    allTags.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                }

                if (lookQuery.TagQuery.AnyTags != null)
                {
                    anyTags.AddRange(lookQuery.TagQuery.AnyTags);
                    anyTags.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                }

                if (allTags.Any())
                {
                    query.And().GroupedAnd(allTags.Select(x => LookConstants.TagsField), allTags.ToArray());
                }

                if (anyTags.Any())
                {
                    query.And().GroupedOr(anyTags.Select(x => LookConstants.TagsField), anyTags.ToArray());
                }
            }

            //// Date
            //if (lookQuery.DateQuery != null && (lookQuery.DateQuery.After.HasValue || lookQuery.DateQuery.Before.HasValue))
            //{
            //    query.And().Range(
            //                    LookConstants.DateField,
            //                    lookQuery.DateQuery.After.HasValue ? lookQuery.DateQuery.After.Value : DateTime.MinValue,
            //                    lookQuery.DateQuery.Before.HasValue ? lookQuery.DateQuery.Before.Value : DateTime.MaxValue);
            //}

            //// Name
            //if (lookQuery.NameQuery != null)
            //{
            // StartsWith
            // Contains
            //}

            // Nodes
            if (lookQuery.NodeQuery != null)
            {
                if (lookQuery.NodeQuery.TypeAliases != null)
                {
                    var typeAliases = new List<string>();

                    typeAliases.AddRange(lookQuery.NodeQuery.TypeAliases);
                    typeAliases.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                    if (typeAliases.Any())
                    {
                        query.And().GroupedOr(typeAliases.Select(x => UmbracoContentIndexer.NodeTypeAliasFieldName), typeAliases.ToArray());
                    }
                }

                if (lookQuery.NodeQuery.ExcludeIds != null)
                {
                    foreach (var excudeId in lookQuery.NodeQuery.ExcludeIds.Distinct())
                    {
                        query.Not().Id(excudeId);
                    }
                }
            }

            try
            {
                searchCriteria = query.Compile();
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(LookService), "Could not compile the Examine query", exception);
            }

            if (searchCriteria != null && searchCriteria is LuceneSearchCriteria)
            {
                Sort sort = null;
                Filter filter = null;

                Func<int, double?> getDistance = x => null;
                Func<string, IHtmlString> getHighlight = null;

                TopDocs topDocs = null;

                switch (lookQuery.SortOn)
                {
                    case SortOn.Date: // newest -> oldest
                        sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField, SortField.LONG, true));
                        break;

                    case SortOn.Name: // a -> z
                        sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.NameField, SortField.STRING));
                        break;
                }

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

                    // raw data for the getDistance func
                    var distances = distanceQueryBuilder.DistanceFilter.Distances;

                    // update getDistance func
                    getDistance = new Func<int, double?>(x =>
                    {
                        if (distances.ContainsKey(x))
                        {
                            return distances[x];
                        }

                        return null;
                    });
                }

                var indexSearcher = new IndexSearcher(((LuceneIndexer)LookService.Indexer).GetLuceneDirectory(), false);

                var luceneSearchCriteria = (LuceneSearchCriteria)searchCriteria;

                // do the Lucene search
                topDocs = indexSearcher.Search(
                                            luceneSearchCriteria.Query, // the query build by Examine
                                            filter ?? new QueryWrapperFilter(luceneSearchCriteria.Query),
                                            LookService.MaxLuceneResults,
                                            sort ?? new Sort(SortField.FIELD_SCORE));

                if (topDocs.TotalHits > 0)
                {
                    // setup the getHightlight func if required
                    if (lookQuery.TextQuery.HighlightFragments > 0 && !string.IsNullOrWhiteSpace(lookQuery.TextQuery.SearchText))
                    {
                        var version = Lucene.Net.Util.Version.LUCENE_29;

                        Analyzer analyzer = new StandardAnalyzer(version);

                        var queryParser = new QueryParser(version, LookConstants.TextField, analyzer);

                        var queryScorer = new QueryScorer(queryParser
                                                            .Parse(lookQuery.TextQuery.SearchText)
                                                            .Rewrite(indexSearcher.GetIndexReader()));

                        Highlighter highlighter = new Highlighter(new SimpleHTMLFormatter("<strong>", "</strong>"), queryScorer);

                        // update the getHightlight func
                        getHighlight = (x) =>
                        {
                            var tokenStream = analyzer.TokenStream(LookConstants.TextField, new StringReader(x));

                            var highlight = highlighter.GetBestFragments(
                                                            tokenStream,
                                                            x,
                                                            lookQuery.TextQuery.HighlightFragments, // max number of fragments
                                                            lookQuery.TextQuery.HighlightSeparator); // fragment separator

                            return new HtmlString(highlight);
                        };
                    }

                    return new EnumerableWithTotal<LookMatch>(
                                                LookSearchService.GetLookMatches(
                                                                    lookQuery,
                                                                    indexSearcher,
                                                                    topDocs,
                                                                    getHighlight,
                                                                    getDistance),
                                                topDocs.TotalHits);
                }
            }            

            return new EnumerableWithTotal<LookMatch>(Enumerable.Empty<LookMatch>(), 0);
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

            // Text
            if (getHighlight != null || getText) // if a highlight function is supplied, then it'll need the text field to process
            {
                fields.Add(LookConstants.TextField);
            }

            fields.Add(LookConstants.TagsField);
            fields.Add(LookConstants.LocationField);

            var mapFieldSelector = new MapFieldSelector(fields.ToArray());

            if (getHighlight == null) // if highlight func does not exist, then create one to always return null
            {
                getHighlight = x => null;
            }

            Func<Field[], string[]> getTags = x =>
            {
                if (x != null)
                {
                    return x
                            .Select(y => y.StringValue())
                            .Where(y => !string.IsNullOrWhiteSpace(y))
                            .ToArray();
                }

                return new string[] { };
            };

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var docId = scoreDoc.doc;

                var doc = indexSearcher.Doc(docId, mapFieldSelector);

                DateTime? date = null;

                if (long.TryParse(doc.Get(LookConstants.DateField), out long ticks))
                {
                    date = new DateTime(ticks);
                }

                var lookMatch = new LookMatch(
                    Convert.ToInt32(doc.Get(LuceneIndexer.IndexNodeIdFieldName)),
                    getHighlight(doc.Get(LookConstants.TextField)),
                    getText ? doc.Get(LookConstants.TextField) : null,
                    getTags(doc.GetFields(LookConstants.TagsField)),
                    date,
                    doc.Get(LookConstants.NameField),
                    doc.Get(LookConstants.LocationField) != null ? new Location(doc.Get(LookConstants.LocationField)) : null,
                    getDistance(docId),
                    scoreDoc.score
                );

                yield return lookMatch;
            }
        }
    }
}
