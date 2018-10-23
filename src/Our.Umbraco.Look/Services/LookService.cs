using Examine;
using Examine.LuceneEngine;
using Examine.LuceneEngine.Providers;
using Examine.Providers;
using Lucene.Net.Spatial.Tier.Projectors;
using Our.Umbraco.Look.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Services
{
    /// <summary>
    /// Useful references:
    /// https://gist.github.com/ismailmayat/3902c660527c8b3d20b38ae724ab9892
    /// http://www.d2digital.co.uk/blog/2015/08/lucenenet-indexer-geospatial-searching-and-umbraco
    /// </summary>
    internal class LookService
    {
        /// <summary>
        /// Function to get text for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, string> TextIndexer { get; set; } = x => LookIndexService.DefaultTextIndexer(x);

        /// <summary>
        /// Function to get the tags for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, string[]> TagIndexer { get; set; } = x => LookIndexService.DefaultTagIndexer(x);

        /// <summary>
        /// Function to get the date for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, DateTime?> DateIndexer { get; set; } = x => LookIndexService.DefaultDateIndexer(x);

        /// <summary>
        /// Function to get the name for the IPublishedContent being indexed
        /// </summary>
        internal Func<IPublishedContent, string> NameIndexer { get; set; } = x => LookIndexService.DefaultNameIndexer(x);

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

        /// <summary>
        /// Setup indexing if configuration valid
        /// </summary>
        /// <param name="gatheringNodeData">indexing event</param>
        /// <param name="umbracoHelper"></param>
        internal static void Initialize(
                                Action<object, DocumentWritingEventArgs, UmbracoHelper> documentWriting,
                                UmbracoHelper umbracoHelper)
        {
            LogHelper.Info(typeof(LookService), "Initializing");

            var valid = true;

            if (LookService.Indexer == null)
            {
                LogHelper.Warn(typeof(LookService), $"Examine Indexer '{LookService.Instance.IndexerName}' Not Found");

                valid = false;
            }
            else if (!(LookService.Indexer is LuceneIndexer))
            {
                // should this ever happen ?

                LogHelper.Warn(typeof(LookService), "Examine Indexer is not of type LuceneIndexer");

                valid = false;
            }

            if (LookService.Searcher == null)
            {
                LogHelper.Warn(typeof(LookService), $"Examine Searcher '{LookService.Instance.SearcherName}' Not Found");

                valid = false;
            }

            if (!valid)
            {
                LogHelper.Warn(typeof(LookService), "Error initializing LookService");
            }
            else
            {
                LogHelper.Info(typeof(LookService), "Indexer & Searcher valid for the LookService");

                // init collection of cartesian tier plotters (and store in singleton)
                IProjector projector = new SinusoidalProjector();
                var plotter = new CartesianTierPlotter(0, projector, CartesianTierPlotter.DefaltFieldPrefix);

                var startTier = plotter.BestFit(LookService.MaxDistance);
                var endTier = plotter.BestFit(1); // min of a 1 mile search

                for (var tier = startTier; tier <= endTier; tier++)
                {
                    LookService.Instance.CartesianTierPlotters.Add(
                        new CartesianTierPlotter(tier, projector, CartesianTierPlotter.DefaltFieldPrefix));
                }

                // wire-up the func
                ((LuceneIndexer)LookService.Indexer).DocumentWriting += (sender, e) => documentWriting(sender, e, umbracoHelper); ;
            }
        }
    }
}