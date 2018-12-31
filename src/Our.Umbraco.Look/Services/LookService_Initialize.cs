using Examine;
using Examine.LuceneEngine.Providers;
using Examine.Providers;
using Lucene.Net.Spatial.Tier.Projectors;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Initialize the look service - it caches reference to all lucene folders & sets up CartesianTierPlotters
        /// </summary>
        internal static void Initialize(UmbracoHelper umbracoHelper)
        {
            if (!LookService.Instance.Initialized)
            {
                lock (LookService.Instance.InitializationLock)
                {
                    if (!LookService.Instance.Initialized)
                    {
                        LogHelper.Info(typeof(LookService), "Initializing...");

                        LookService.Instance.UmbracoHelper = umbracoHelper;

                        IndexProviderCollection indexProviderCollection = null;

                        try
                        {
                            indexProviderCollection = ExamineManager.Instance.IndexProviderCollection;
                        }
                        catch
                        {
                            // running outside of Umbraco - in unit test context
                        }

                        if (indexProviderCollection != null)
                        {
                            var indexProviders = indexProviderCollection
                                                    .Select(x => x as LuceneIndexer)
                                                    .Where(x => x != null)
                                                    .ToArray();

                            // cache the collection of Lucene Directory objs (so don't have to at query time)
                            LookService.Instance.IndexSetDirectories = indexProviders.ToDictionary(x => x.IndexSetName, x => x.GetLuceneDirectory());
                        }

                        // init collection of cartesian tier plotters
                        IProjector projector = new SinusoidalProjector();
                        var plotter = new CartesianTierPlotter(0, projector, LookConstants.LocationTierFieldPrefix);

                        var startTier = plotter.BestFit(LookService.MaxDistance);
                        var endTier = plotter.BestFit(1); // min of a 1 mile search

                        for (var tier = startTier; tier <= endTier; tier++)
                        {
                            LookService
                                .Instance
                                .CartesianTierPlotters
                                .Add(new CartesianTierPlotter(
                                                    tier,
                                                    projector,
                                                    LookConstants.LocationTierFieldPrefix));
                        }

                        LookService.Instance.Initialized = true;
                    }
                }
            }
        }
    }
}
