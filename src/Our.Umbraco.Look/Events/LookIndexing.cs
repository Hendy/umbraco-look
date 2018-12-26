using Examine;
using Lucene.Net.Documents;
using Our.Umbraco.Look.Extensions;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Our.Umbraco.Look.Events
{
    /// <summary>
    /// Enables the indexing of inner IPublishedContent collections on a node
    /// By default, Examine will create 1 Lucene document for a Node,
    /// using this Indexer will create 1 Lucene document for each descendant IPublishedContent collection of a node (eg, all NestedContent, or StackedContent items)
    /// </summary>
    public class LookIndexing : ApplicationEventHandler
    {
        /// <summary>
        /// Ask Examine if it has any LookDetachedIndexers (as they may be registered at runtime in future)
        /// </summary>
        private LookIndexer[] _lookIndexers;

        /// <summary>
        /// shared umbraco helper
        /// </summary>
        private UmbracoHelper _umbracoHelper;

        /// <summary>
        /// Event used to maintain any detached indexes, as and when Umbraco data changes
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

                ContentService.Saved += this.ContentService_Saved;
                MediaService.Saved += this.MediaService_Saved;
                MemberService.Saved += this.MemberService_Saved;

                ContentService.Deleted += this.ContentService_Deleted;
                MediaService.Deleted += this.MediaService_Deleted;
                MemberService.Deleted += this.MemberService_Deleted;
            }
        }


        private void ContentService_Saved(IContentService sender, SaveEventArgs<IContent> e)
        {         
            foreach (var entity in e.SavedEntities)
            {
                this.Save(this._umbracoHelper.TypedContent(entity.Id));
            }
        }

        private void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            foreach (var entity in e.SavedEntities)
            {
                this.Save(this._umbracoHelper.TypedMedia(entity.Id));
            }
        }

        private void MemberService_Saved(IMemberService sender, SaveEventArgs<IMember> e)
        {
            foreach (var entity in e.SavedEntities)
            {
                this.Save(this._umbracoHelper.TypedMember(entity.Id));
            }
        }

        private void ContentService_Deleted(IContentService sender, DeleteEventArgs<IContent> e)
        {
            foreach (var entity in e.DeletedEntities)
            {
                this.Delete(entity.Id);
            }
        }

        private void MediaService_Deleted(IMediaService sender, DeleteEventArgs<IMedia> e)
        {
            foreach (var entity in e.DeletedEntities)
            {
                this.Delete(entity.Id);
            }
        }

        private void MemberService_Deleted(IMemberService sender, DeleteEventArgs<IMember> e)
        {
            foreach (var entity in e.DeletedEntities)
            {
                this.Delete(entity.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedContent"></param>
        private void Save(IPublishedContent publishedContent)
        {
            if (publishedContent == null) return; // content may have been saved, but not yet published

            foreach (var lookIndexer in this._lookIndexers)
            {
                var indexWriter = lookIndexer.GetIndexWriter();

                var indexingContext = new IndexingContext(null, publishedContent, lookIndexer.Name);
                
                var document = new Document();

                LookService.Index(indexingContext, document);

                indexWriter.AddDocument(document);  // index the content, media or memeber node itself

                foreach (var detachedContent in publishedContent.GetFlatDetachedDescendants())
                {
                    indexingContext = new IndexingContext(publishedContent, detachedContent, lookIndexer.Name);

                    document = new Document();

                    LookService.Index(indexingContext, document);

                    indexWriter.AddDocument(document); // index each detached item
                }

                indexWriter.Optimize();

                indexWriter.Commit();
            }
        }

        private void Delete(int id)
        {
            // TODO: dete from index where host id = id
        }
    }
}
