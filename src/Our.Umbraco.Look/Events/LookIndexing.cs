﻿using Examine;
using Lucene.Net.Index;
using Our.Umbraco.Look.Services;
using System.ComponentModel;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Enables the indexing of inner IPublishedContent collections on a node
    /// By default, Examine will create 1 Lucene document for a Node,
    /// using this Indexer will create 1 Lucene document for each descendant IPublishedContent collection of a node (eg, all NestedContent, or StackedContent items)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] // hide from api intellisense
    public class LookIndexing : ApplicationEventHandler
    {
        /// <summary>
        /// Collection of LookIndexers registered with Examine
        /// </summary>
        private LookIndexer[] _lookIndexers;

        /// <summary>
        /// shared umbraco helper
        /// </summary>
        private UmbracoHelper _umbracoHelper;

        /// <summary>
        /// Wire-up events to maintain Look indexes 'as and when' Umbraco data changes
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // set once, as they will be configured in the config
            this._lookIndexers = ExamineManager
                                        .Instance
                                        .IndexProviderCollection
                                        .Select(x => x as LookIndexer)
                                        .Where(x => x != null)
                                        .ToArray();

            if (this._lookIndexers.Any())
            {
                this._umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                LookService.Initialize(this._umbracoHelper); 

                ContentService.Published += ContentService_Published;
                MediaService.Saved += this.MediaService_Saved;
                MemberService.Saved += this.MemberService_Saved;

                ContentService.UnPublished += ContentService_UnPublished;
                MediaService.Deleted += this.MediaService_Deleted;
                MemberService.Deleted += this.MemberService_Deleted;
            }
        }

        private void ContentService_Published(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            foreach (var entity in e.PublishedEntities)
            {
                this.Update(this._umbracoHelper.TypedContent(entity.Id));
            }
        }

        private void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            foreach (var entity in e.SavedEntities)
            {
                this.Update(this._umbracoHelper.TypedMedia(entity.Id));
            }
        }

        private void MemberService_Saved(IMemberService sender, SaveEventArgs<IMember> e)
        {
            foreach (var entity in e.SavedEntities)
            {
                this.Update(this._umbracoHelper.TypedMember(entity.Id));
            }
        }

        private void ContentService_UnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            foreach (var entity in e.PublishedEntities)
            {
                this.Remove(entity.Id);
            }
        }

        private void MediaService_Deleted(IMediaService sender, DeleteEventArgs<IMedia> e)
        {
            foreach (var entity in e.DeletedEntities)
            {
                this.Remove(entity.Id);
            }
        }

        private void MemberService_Deleted(IMemberService sender, DeleteEventArgs<IMember> e)
        {
            foreach (var entity in e.DeletedEntities)
            {
                this.Remove(entity.Id);
            }
        }

        /// <summary>
        /// Update the Lucene document in all indexes
        /// </summary>
        /// <param name="publishedContent"></param>
        private void Update(IPublishedContent publishedContent)
        {
            if (publishedContent == null) return; // content may have been saved, but not yet published

            this.Remove(publishedContent.Id);

            foreach (var lookIndexer in this._lookIndexers)
            {
                lookIndexer.Index(new IPublishedContent[] { publishedContent });
            }
        }

        /// <summary>
        /// Delete from all indexes all Lucene documents where the id identifies 
        /// the Umbraco Content, Media or Member item and any of its Detached items
        /// </summary>
        /// <param name="id">The Umbraco Content, Media or Member Id (Detached items do not have Ids)</param>
        private void Remove(int id)
        {
            foreach (var lookIndexer in this._lookIndexers)
            {
                var indexWriter = lookIndexer.GetIndexWriter();

                indexWriter.DeleteDocuments(new Term[] {
                    new Term(LookConstants.NodeIdField, id.ToString()), // the actual item
                    new Term(LookConstants.HostIdField, id.ToString()) // any detached items
                });

                indexWriter.Commit();
            }
        }
    }
}
