using Examine;
using Examine.LuceneEngine.Providers;
using Examine.Providers;
using Lucene.Net.Analysis;
using Lucene.Net.Spatial.Tier.Projectors;
using Our.Umbraco.Look.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    /// <summary>
    /// Useful references:
    /// https://gist.github.com/ismailmayat/3902c660527c8b3d20b38ae724ab9892
    /// http://www.d2digital.co.uk/blog/2015/08/lucenenet-indexer-geospatial-searching-and-umbraco
    /// </summary>
    public partial class LookService
    {
        /// <summary>
        /// Function to get the name for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, string> NameIndexer { get; set; } = x => LookService.DefaultNameIndexer(x);

        /// <summary>
        /// Function to get the date for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, DateTime?> DateIndexer { get; set; } = x => LookService.DefaultDateIndexer(x);

        /// <summary>
        /// Function to get text for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, string> TextIndexer { get; set; } = x => LookService.DefaultTextIndexer(x);

        /// <summary>
        /// Function to get the tags for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, string[]> TagIndexer { get; set; } = x => LookService.DefaultTagIndexer(x);

        /// <summary>
        /// Function to get a location for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, Location> LocationIndexer { get; set; } = x => null;

        /// <summary>
        /// Collection of cartesian tier plotters
        /// </summary>
        internal List<CartesianTierPlotter> CartesianTierPlotters { get; } = new List<CartesianTierPlotter>();

        /// <summary>
        /// Name of indexer to use (from configuration)
        /// </summary>
        private string IndexerName { get; }

        /// <summary>
        /// Name of searcher to use (from configuration)
        /// </summary>
        private string SearcherName { get; }

        /// <summary>
        /// Gets the Examine indexer
        /// </summary>
        internal static BaseIndexProvider Indexer => ExamineManager.Instance.IndexProviderCollection[LookService.Instance.IndexerName];

        /// <summary>
        /// Get the Analyzer in use by Examine
        /// </summary>
        internal static Analyzer Analyzer => ((LuceneIndexer)LookService.Indexer).IndexingAnalyzer;

        /// <summary>
        /// Gets the Examine searcher
        /// </summary>
        internal static BaseSearchProvider Searcher => ExamineManager.Instance.SearchProviderCollection[LookService.Instance.SearcherName];

        /// <summary>
        /// Max distance in miles for distance searches & location indexing
        /// </summary>
        internal static double MaxDistance => 10000; // 12450 = half circumfrence of earth TODO: make configuration

        /// <summary>
        /// max numnber of results to request for a lucene query
        /// </summary>
        internal static int MaxLuceneResults => 5000; // TODO: make configurable (maybe part of the SearchQuery obj)

        /// <summary>
        /// Access the singleton instance of this search service
        /// </summary>
        internal static LookService Instance => _lazy.Value;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static readonly Lazy<LookService> _lazy = new Lazy<LookService>(() => new LookService());

        /// <summary>
        /// Singleton constructor (used privately to maintain state for consumer registered indexer functions)
        /// </summary>
        private LookService()
        {
            var configuredIndexerName = ConfigurationManager.AppSettings["Our.Umbraco.Look.IndexerName"];
            var configuredSearcherName = ConfigurationManager.AppSettings["Our.Umbraco.Look.SearcherName"];

            this.IndexerName = configuredIndexerName ?? "ExternalIndexer";
            this.SearcherName = configuredSearcherName ?? "ExternalSearcher";
        }
    }
}