using Examine;
using Examine.LuceneEngine;
using Our.Umbraco.Look.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;
using UmbracoExamine;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Set all examine indexers (this may be called from the Hook indexing startup event)
        /// </summary>
        internal static void SetExamineIndexers()
        {
            // get all examine indexer names
            var examineIndexerNames = ExamineManager
                                        .Instance
                                        .IndexProviderCollection
                                        .Select(x => x as BaseUmbracoIndexer) // UmbracoContentIndexer, UmbracoMemberIndexer
                                        .Where(x => x != null)
                                        .Select(x => x.Name)
                                        .ToArray();

            LookService.SetExamineIndexers(examineIndexerNames);
        }

        /// <summary>
        /// Set the supplied examine indexers (this may be called by the consumer to specify the Examine indexes to hook into)
        /// </summary>
        /// <param name="examineIndexers">names of Examine indexers to hook into (null or empty array = none)</param>
        internal static void SetExamineIndexers(string[] examineIndexerNames)
        {
            LookService.Instance._examineIndexersConfigured = true; // set flag so that hook indexing startup event doens't reset any conumser set configuration

            // all examine indexers that should be hooked into (string key = indexer name)
            var examineIndexers = new Dictionary<string, BaseUmbracoIndexer>(); // default to empty - ie, no examine indexers to hook into

            if (examineIndexerNames != null && examineIndexerNames.Any())
            {
                // we (might) have indexers to hook into
                examineIndexers = ExamineManager
                                    .Instance
                                    .IndexProviderCollection
                                    .Select(x => x as BaseUmbracoIndexer) // UmbracoContentIndexer, UmbracoMemberIndexer
                                    .Where(x => x != null)
                                    .Where(x => examineIndexerNames.Contains(x.Name))
                                    .ToDictionary(x => x.Name, x => x);
            }

            // if there are indexers already registered, remove those that are not in the collection
            var removeEvents = LookService
                                .Instance
                                ._examineDocumentWritingEvents
                                .Where(x => !examineIndexers.ContainsKey(x.Key))
                                .ToDictionary(x => x.Key, x => x.Value);

            foreach(var removeEvent in removeEvents)
            {
                var indexer = ExamineManager.Instance.IndexProviderCollection[removeEvent.Key] as BaseUmbracoIndexer;

                indexer.DocumentWriting -= removeEvent.Value;

                LookService.Instance._examineDocumentWritingEvents.Remove(removeEvent.Key);
            }

            // add events if not already registered
            foreach(var examineIndexer in examineIndexers)
            {
                if (!LookService.Instance._examineDocumentWritingEvents.ContainsKey(examineIndexer.Key))
                {
                    EventHandler<DocumentWritingEventArgs> addEvent = (sender, e) => LookService.DocumentWriting(sender, e, examineIndexer.Key);

                    LookService.Instance._examineDocumentWritingEvents[examineIndexer.Key] = addEvent;

                    examineIndexers[examineIndexer.Key].DocumentWriting += addEvent;
                }

                LogHelper.Info(typeof(LookService), $"Hooking into Examine indexer '{ examineIndexer.Key }'");
            }
        }

        /// <summary>
        /// TODO: revist to make more efficient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="indexerName">Name of the indexer for which this DocumentWriting event is being executed on</param>
        private static void DocumentWriting(object sender, DocumentWritingEventArgs e, string indexerName)
        {
            IPublishedContent publishedContent = null;
            PublishedItemType? publishedItemType = null;

            if (LookService.Instance._umbracoHelper == null)
            {
                throw new Exception("Unexpected null value for UmbracoHelper - Look not initialized");
            }

            publishedContent = LookService.Instance._umbracoHelper.TypedContent(e.NodeId);

            if (publishedContent != null)
            {
                publishedItemType = PublishedItemType.Content;
            }
            else // attempt to get as media
            {                
                publishedContent = LookService.Instance._umbracoHelper.TypedMedia(e.NodeId);

                if (publishedContent != null)
                {
                    publishedItemType = PublishedItemType.Media;
                }
                else // attempt to get as member
                {                    
                    publishedContent = LookService.Instance._umbracoHelper.SafeTypedMember(e.NodeId);

                    if (publishedContent != null)
                    {
                        publishedItemType = PublishedItemType.Member;
                    }
                }
            }

            if (publishedContent != null)
            {
                var indexerConfiguration = LookConfiguration.IndexerConfiguration[indexerName] ?? IndexerConfiguration.GetDefaultIndexerConfiguration();

                if (publishedItemType == PublishedItemType.Content && indexerConfiguration.IndexContent
                    || publishedItemType == PublishedItemType.Media && indexerConfiguration.IndexMedia
                    || publishedItemType == PublishedItemType.Member && indexerConfiguration.IndexMembers)

                {
                    var indexingContext = new IndexingContext(
                            hostNode: null,
                            node: publishedContent,
                            indexerName: indexerName);

                    LookService.EnsureContext();

                    LookService.Index(indexingContext, e.Document);
                }
            }
        }
    }
}
