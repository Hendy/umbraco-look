using Examine;
using Examine.LuceneEngine.Providers;
using Examine.Providers;
using Lucene.Net.Spatial.Tier.Projectors;
using System;
using System.Collections.Generic;
using System.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Web;

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
        /// Name of indexer to use (from configuration)
        /// </summary>
        private string IndexerName { get; }

        /// <summary>
        /// Name of searcher to use (from configuration)
        /// </summary>
        private string SearcherName { get; }

        /// <summary>
        /// 
        /// </summary>
        private List<CartesianTierPlotter> CartesianTierPlotters { get; } = new List<CartesianTierPlotter>();

        /// <summary>
        /// Access the singleton instance of this search service
        /// </summary>
        private static LookService Instance => _lazy.Value;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static readonly Lazy<LookService> _lazy = new Lazy<LookService>(() => new LookService());

        /// <summary>
        /// Gets the Examine indexer
        /// </summary>
        private static BaseIndexProvider Indexer => ExamineManager.Instance.IndexProviderCollection[LookService.Instance.IndexerName];

        /// <summary>
        /// Gets the Examine searcher
        /// </summary>
        private static BaseSearchProvider Searcher => ExamineManager.Instance.SearchProviderCollection[LookService.Instance.SearcherName];

        /// <summary>
        /// Gets the field name to use for the text - this field is expected to contain a sizeable amount of text
        /// </summary>
        private static string TextField => "Our.Umbraco.Look_Text";

        /// <summary>
        /// Gets the field name to use for the tags - this field will contain space delimited non-tokenizable strings
        /// </summary>
        private static string TagsField => "Our.Umbraco.Look_Tags";

        /// <summary>
        /// Gets the field name to use for the date - this fieldswill  stores the date as the number of seconds from the year 2000 (so it's a number that can be sorted)
        /// </summary>
        private static string DateField => "Our.Umbraco.Look_Date";

        /// <summary>
        /// Gets the field name to use for the name
        /// </summary>
        private static string NameField => "Our.Umbraco.Look_Name";

        /// <summary>
        /// Gets the field name to use for the location
        /// </summary>
        private static string LocationField => "Our.Umbraco.Look_Location";

        /// <summary>
        /// not stored in index, but used as a result field
        /// </summary>
        private static string DistanceField => "Our.Umbraco.Look_Distance";

        /// <summary>
        /// Max distance in miles for distance searches & location indexing
        /// </summary>
        private static double MaxDistance => 10000; // 12450 = half circumfrence of earth TODO: make configuration

        /// <summary>
        /// max numnber of results to request for a lucene query
        /// </summary>
        private static int MaxLuceneResults => 5000; // TODO: make configurable (maybe part of the SearchQuery obj)

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
        /// Wire up the indexing event if both the configured indexer & searcher found
        /// </summary>
        /// <param name="action">indexing event</param>
        /// <param name="umbracoHelper"></param>
        internal static void Initialize(Action<object, IndexingNodeDataEventArgs, UmbracoHelper> action, UmbracoHelper umbracoHelper)
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

                // wire-up events
                LookService.Indexer.GatheringNodeData += (sender, e) => action(sender, e, umbracoHelper);

                ((LuceneIndexer)LookService.Indexer).DocumentWriting += DocumentWriting;
            }
        }
    }
}
