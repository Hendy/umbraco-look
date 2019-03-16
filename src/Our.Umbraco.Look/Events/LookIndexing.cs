using Examine;
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
            this.Update(e.PublishedEntities.Select(x => this._umbracoHelper.TypedContent(x.Id)).ToArray());
        }

        private void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            this.Update(e.SavedEntities.Select(x => this._umbracoHelper.TypedMedia(x.Id)).ToArray());
        }

        private void MemberService_Saved(IMemberService sender, SaveEventArgs<IMember> e)
        {
            this.Update(e.SavedEntities.Select(x => this._umbracoHelper.TypedMember(x.Id)).ToArray());
        }

        private void ContentService_UnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            this.Remove(e.PublishedEntities.Select(x => x.Id).ToArray());
        }

        private void MediaService_Deleted(IMediaService sender, DeleteEventArgs<IMedia> e)
        {
            this.Remove(e.DeletedEntities.Select(x => x.Id).ToArray());
        }

        private void MemberService_Deleted(IMemberService sender, DeleteEventArgs<IMember> e)
        {
            this.Remove(e.DeletedEntities.Select(x => x.Id).ToArray());
        }

        /// <summary>
        /// Update the Lucene document in all indexes
        /// </summary>
        /// <param name="publishedContentItems"></param>
        private void Update(IPublishedContent[] publishedContentItems)
        {
            if (publishedContentItems == null || !publishedContentItems.Any()) return;

            this.Remove(publishedContentItems.Select(x => x.Id).ToArray());

            foreach(var lookIndexer in this._lookIndexers)
            {
                lookIndexer.Index(publishedContentItems);
            }
        }

        /// <summary>
        /// Delete from all indexes all Lucene documents where the id identifies 
        /// the Umbraco Content, Media or Member item and any of its Detached items
        /// </summary>
        /// <param name="ids">The Umbraco Content, Media or Member Ids (Detached items do not have Ids)</param>
        private void Remove(int[] ids)
        {
            foreach (var lookIndexer in this._lookIndexers)
            {
                var indexWriter = lookIndexer.GetIndexWriter();

                foreach (var id in ids)
                {
                    indexWriter.DeleteDocuments(new Term[] {
                        new Term(LookConstants.NodeIdField, id.ToString()), // the actual item
                        new Term(LookConstants.HostIdField, id.ToString()) // any detached items
                    });
                }

                indexWriter.Commit();
            }
        }
    }
}
