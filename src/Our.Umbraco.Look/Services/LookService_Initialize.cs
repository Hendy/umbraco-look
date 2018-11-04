using Examine;
using Examine.LuceneEngine;
using Examine.LuceneEngine.Providers;
using Examine.Providers;
using Lucene.Net.Spatial.Tier.Projectors;
using System;
using Umbraco.Core.Logging;
using Umbraco.Web;

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

            // read exmaine configuration for Indexers / Searchers  & which to use as default
            //ExamineManager.Instance.IndexProviderCollection[LookService.Instance.IndexerName]


            // ideally should have index registrations known that this point, so we know which examine indexers require the custom fields

            // -----

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
                    LookService
                        .Instance
                        .CartesianTierPlotters
                        .Add(new CartesianTierPlotter(
                                            tier,
                                            projector,
                                            CartesianTierPlotter.DefaltFieldPrefix));
                }

                // wire-up the func
                ((LuceneIndexer)LookService.Indexer).DocumentWriting += (sender, e) => documentWriting(sender, e, umbracoHelper);


                // TODO: wire up to all examine indexes (unless config says not to ? - could force all registartions to have taken place at this point in time ?)
            }
        }
    }
}
