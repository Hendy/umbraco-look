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
                                Action<object, DocumentWritingEventArgs, UmbracoHelper, string> documentWriting,
                                UmbracoHelper umbracoHelper)
        {
            LogHelper.Info(typeof(LookService), "Initializing...");

            var indexProviders = ExamineManager
                                    .Instance
                                    .IndexProviderCollection
                                    .Select(x => x as BaseUmbracoIndexer) // UmbracoContentIndexer, UmbracoMemberIndexer
                                    .Where(x => x != null)
                                    .ToArray();
            
            if (!indexProviders.Any())
            {
                LogHelper.Warn(typeof(LookService), "Unable to initialize indexing as could not find any Umbraco Examine indexers !");

                return;
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

            // cache the collection of Lucene Directory objs (so don't have to at query time)
            LookService.Instance.IndexSetDirectories = indexProviders.ToDictionary(x => x.IndexSetName, x => x.GetLuceneDirectory());

            // hook into all index providers
            foreach(var indexProvider in indexProviders)
            {
                indexProvider.DocumentWriting += (sender, e) => documentWriting(sender, e, umbracoHelper, indexProvider.Name);
            }
        }
    }
}
