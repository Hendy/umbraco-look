using Examine;
using Lucene.Net.Index;
using Lucene.Net.Search;
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

            this.Delete(publishedContent.Id);

            foreach (var lookIndexer in this._lookIndexers)
            {
                lookIndexer.Index(new IPublishedContent[] { publishedContent });
            }
        }

        /// <summary>
        /// Delete all lucene documents where node (item) id or deteached items using host id = the supplied id
        /// </summary>
        /// <param name="id"></param>
        private void Delete(int id)
        {
            // build query
            var query = new BooleanQuery();
            var idQuery = new BooleanQuery();

            idQuery.Add(new TermQuery(new Term(LookConstants.NodeIdField, id.ToString())), BooleanClause.Occur.SHOULD);
            idQuery.Add(new TermQuery(new Term(LookConstants.HostIdField, id.ToString())), BooleanClause.Occur.SHOULD);

            query.Add(idQuery, BooleanClause.Occur.MUST); 

            foreach (var lookIndexer in this._lookIndexers)
            {
                lookIndexer.GetIndexWriter().DeleteDocuments(query);
            }
        }
    }
}
