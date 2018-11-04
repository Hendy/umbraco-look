using Examine;
using Examine.LuceneEngine;
using Lucene.Net.Spatial.Tier.Projectors;
using System;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Web;
using UmbracoExamine;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
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

            var indexProviders = ExamineManager
                                    .Instance
                                    .IndexProviderCollection
                                    .Select(x => x as BaseUmbracoIndexer)
                                    .Where(x => x != null)
                                    .Select(x => (BaseUmbracoIndexer)x) // UmbracoContentIndexer, UmbracoMemberIndexer
                                    .ToArray();

            if (!indexProviders.Any())
            {
                LogHelper.Warn(typeof(LookService), "Unable to initialize as could not find any Umbraco Examine indexers !");
            }
            else
            {
                // init collection of cartesian tier plotters (and store in singleton)
                IProjector projector = new SinusoidalProjector();
                var plotter = new CartesianTierPlotter(0, projector, CartesianTierPlotter.DefaltFieldPrefix);

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
                                            CartesianTierPlotter.DefaltFieldPrefix));
                }

                // hook into all index providers
                foreach(var indexProvider in indexProviders)
                {
                    indexProvider.DocumentWriting += (sender, e) => documentWriting(sender, e, umbracoHelper);
                }
            }
        }
    }
}
