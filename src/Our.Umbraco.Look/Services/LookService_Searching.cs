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
    /// <summary>
    /// 
    /// </summary>
    public partial class LookService
    {
        /// <summary>
        ///  Main searching method
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns>an IEnumerableWithTotal</returns>
        public static IEnumerableWithTotal<SearchMatch> Search(SearchQuery searchQuery)
        {
            IEnumerableWithTotal<SearchMatch> searchMatches = null; // prepare return value

            if (searchQuery == null)
            {
                LogHelper.Warn(typeof(LookService), "Supplied search query was null");
            }
            else
            {
                var searchProvider = LookService.Searcher;

                var searchCriteria = searchProvider.CreateSearchCriteria();

                var query = searchCriteria.Field(string.Empty, string.Empty);

                // Text
                if (!string.IsNullOrWhiteSpace(searchQuery.TextQuery.SearchText))
                {
                    if (searchQuery.TextQuery.Fuzzyness > 0)
                    {
                        query.And().Field(LookService.TextField, searchQuery.TextQuery.SearchText.Fuzzy(searchQuery.TextQuery.Fuzzyness));
                    }
                    else
                    {
                        query.And().Field(LookService.TextField, searchQuery.TextQuery.SearchText);
                    }
                }

                // Tags
                if (searchQuery.TagQuery != null)
                {
                    var allTags = new List<string>();
                    var anyTags = new List<string>();

                    if (searchQuery.TagQuery.AllTags != null)
                    {
                        allTags.AddRange(searchQuery.TagQuery.AllTags);
                        allTags.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                    }

                    if (searchQuery.TagQuery.AnyTags != null)
                    {
                        anyTags.AddRange(searchQuery.TagQuery.AnyTags);
                        anyTags.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                    }

                    if (allTags.Any())
                    {
                        query.And().GroupedAnd(allTags.Select(x => LookService.TagsField), allTags.ToArray());
                    }

                    if (anyTags.Any())
                    {
                        query.And().GroupedOr(allTags.Select(x => LookService.TagsField), anyTags.ToArray());
                    }
                }

                // TODO: Date

                // TODO: Name

                // Nodes
                if (searchQuery.NodeQuery != null)
                {
                    if (searchQuery.NodeQuery.TypeAliases != null)
                    {
                        var typeAliases = new List<string>();

                        typeAliases.AddRange(searchQuery.NodeQuery.TypeAliases);
                        typeAliases.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                        if (typeAliases.Any())
                        {
                            query.And().GroupedOr(typeAliases.Select(x => UmbracoContentIndexer.NodeTypeAliasFieldName), typeAliases.ToArray());
                        }
                    }

                    if (searchQuery.NodeQuery.ExcludeIds != null)
                    {
                        foreach (var excudeId in searchQuery.NodeQuery.ExcludeIds.Distinct())
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

                    switch (searchQuery.SortOn)
                    {
                        case SortOn.Date: // newest -> oldest
                            sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookService.DateField, SortField.LONG, true));
                            break;

                        case SortOn.Name: // a -> z
                            sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookService.NameField, SortField.STRING));
                            break;
                    }

                    if (searchQuery.LocationQuery != null && searchQuery.LocationQuery.Location != null && searchQuery.LocationQuery.MaxDistance != null)
                    {
                        var distanceQueryBuilder = new DistanceQueryBuilder(
                                                    searchQuery.LocationQuery.Location.Latitude,
                                                    searchQuery.LocationQuery.Location.Longitude,
                                                    searchQuery.LocationQuery.MaxDistance.GetMiles(),
                                                    LookService.LocationField + "_Latitude",
                                                    LookService.LocationField + "_Longitude",
                                                    CartesianTierPlotter.DefaltFieldPrefix,
                                                    true);

                        // update filter
                        filter = distanceQueryBuilder.Filter;

                        if (searchQuery.SortOn == SortOn.Distance)
                        {
                            // update sort
                            sort = new Sort(
                                        new SortField(
                                            LookService.DistanceField,
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

                    // Do the Lucene search
                    topDocs = indexSearcher.Search(
                                                luceneSearchCriteria.Query, // the query build by Examine
                                                filter ?? new QueryWrapperFilter(luceneSearchCriteria.Query),
                                                LookService.MaxLuceneResults,
                                                sort ?? new Sort(SortField.FIELD_SCORE));

                    if (topDocs.TotalHits > 0)
                    {
                        // setup the highlighing func if required
                        if (searchQuery.TextQuery.HighlightFragments > 0 && !string.IsNullOrWhiteSpace(searchQuery.TextQuery.SearchText))
                        {
                            var version = Lucene.Net.Util.Version.LUCENE_29;

                            Analyzer analyzer = new StandardAnalyzer(version);

                            var queryParser = new QueryParser(version, LookService.TextField, analyzer);

                            var queryScorer = new QueryScorer(queryParser
                                                                .Parse(searchQuery.TextQuery.SearchText)
                                                                .Rewrite(indexSearcher.GetIndexReader()));

                            Highlighter highlighter = new Highlighter(new SimpleHTMLFormatter("<strong>", "</strong>"), queryScorer);

                            // update the func so it does real highlighting work
                            getHighlight = (x) =>
                            {
                                var tokenStream = analyzer.TokenStream(LookService.TextField, new StringReader(x));

                                var highlight = highlighter.GetBestFragments(
                                                                tokenStream,
                                                                x,
                                                                searchQuery.TextQuery.HighlightFragments, // max number of fragments
                                                                searchQuery.TextQuery.HighlightSeparator); // fragment separator

                                return new HtmlString(highlight);
                            };
                        }

                        searchMatches = new EnumerableWithTotal<SearchMatch>(
                                                    LookService.GetSearchMatches(
                                                                        searchQuery,
                                                                        indexSearcher,
                                                                        topDocs,
                                                                        getHighlight,
                                                                        getDistance),
                                                    topDocs.TotalHits);
                    }
                }
            }

            return searchMatches ?? new EnumerableWithTotal<SearchMatch>(Enumerable.Empty<SearchMatch>(), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexSearcher"></param>
        /// <param name="topDocs"></param>
        /// <param name="getHighlight"></param>
        /// <param name="getDistance"></param>
        /// <returns></returns>
        private static IEnumerable<SearchMatch> GetSearchMatches(
                                                    SearchQuery searchQuery,
                                                    IndexSearcher indexSearcher,
                                                    TopDocs topDocs,
                                                    Func<string, IHtmlString> getHighlight,
                                                    Func<int, double?> getDistance)
        {
            bool getText = searchQuery.TextQuery != null && searchQuery.TextQuery.GetText;
            bool getTags = searchQuery.TagQuery != null && searchQuery.TagQuery.GetTags;

            var fields = new List<string>();

            fields.Add(LuceneIndexer.IndexNodeIdFieldName); // "__NodeId"
            fields.Add(LookService.DateField);
            fields.Add(LookService.NameField);
            fields.Add(LookService.LocationField);

            /// if a highlight function is supplied, then it'll need the text field to process
            if (getHighlight != null || getText)
            {
                fields.Add(LookService.TextField);
            }

            if (getHighlight == null) // if highlight func doens't exist, then create one to always return null
            {
                getHighlight = x => null;
            }

            if (getTags)
            {
                fields.Add(LookService.TagsField);
            }

            var mapFieldSelector = new MapFieldSelector(fields.ToArray());

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var docId = scoreDoc.doc;

                var doc = indexSearcher.Doc(docId, mapFieldSelector);

                DateTime? date = null;

                if (long.TryParse(doc.Get(LookService.DateField), out long ticks))
                {
                    date = new DateTime(ticks);
                }

                var searchMatch = new SearchMatch()
                {
                    Id = Convert.ToInt32(doc.Get(LuceneIndexer.IndexNodeIdFieldName)),
                    Highlight = getHighlight(doc.Get(LookService.TextField)),
                    Text = getText ? doc.Get(LookService.TextField) : null,
                    Tags = getTags ? doc.Get(LookService.TagsField).Split(' ') : null,
                    Date = date,
                    Name = doc.Get(LookService.NameField),
                    Location = doc.Get(LookService.LocationField) != null ? new Location(doc.Get(LookService.LocationField)) : null,
                    Distance = getDistance(docId),
                    Score = scoreDoc.score
                };

                yield return searchMatch;
            }
        }
    }
}
