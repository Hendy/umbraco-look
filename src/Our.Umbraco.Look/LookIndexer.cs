using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Our.Umbraco.Look.Extensions;
using Our.Umbraco.Look.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look
{
    public class LookIndexer : LuceneIndexer
    {
        private UmbracoHelper _umbracoHelper = null;

        private UmbracoHelper UmbracoHelper
        {
            get
            {
                if (this._umbracoHelper == null)
                {
                    this._umbracoHelper = new UmbracoHelper(UmbracoContext.Current);  
                }

                return this._umbracoHelper;
            }
        }

        //private IndexerConfiguration IndexerConfiguration => LookService.GetIndexerConfiguration(this.Name);

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        /// <summary>
        /// Triggered (via base.RebuildIndex) when for Umbraco back office rebuild event
        /// </summary>
        protected override void PerformIndexRebuild()
        {
            var indexerConfiguration = LookService.GetIndexerConfiguration(this.Name);
                                                                                            
            if (indexerConfiguration.ShouldIndexContent || indexerConfiguration.ShouldIndexDetachedContent)
            {
                var content = this.UmbracoHelper.TypedContentAtXPath("//*[@isDoc]");

                this.Index(
                        content, 
                        indexerConfiguration.ShouldIndexContent, 
                        indexerConfiguration.ShouldIndexDetachedContent);
            }

            if (indexerConfiguration.ShouldIndexMedia || indexerConfiguration.ShouldIndexDetachedMedia)
            {
                var media = this.UmbracoHelper
                                .TypedMediaAtRoot()
                                .SelectMany(x => x.DescendantsOrSelf());

                this.Index(
                        media, 
                        indexerConfiguration.ShouldIndexMedia, 
                        indexerConfiguration.ShouldIndexDetachedMedia);
            }

            if (indexerConfiguration.ShouldIndexMembers || indexerConfiguration.ShouldIndexDetachedMembers)
            {
                var members = ApplicationContext
                                .Current
                                .Services
                                .MemberService
                                .GetAll(0, int.MaxValue, out int totalRecords)
                                .Select(x => this.UmbracoHelper.TypedMember(x.Id));

                this.Index(
                        members, 
                        indexerConfiguration.ShouldIndexMembers, 
                        indexerConfiguration.ShouldIndexDetachedMembers);
            }

            this.GetIndexWriter().Optimize();

            this.OnIndexOperationComplete(new EventArgs()); // causes the backoffice rebuild to end (UI thread watches a cache value)
        }

        protected override void PerformIndexAll(string type)
        {
        }

        /// <summary>
        /// Index the supplied nodes
        /// </summary>
        /// <param name="nodes"></param>
        public void Index(IEnumerable<IPublishedContent> nodes)
        {
            var indexerConfiguration = LookService.GetIndexerConfiguration(this.Name);

            nodes = nodes.Where(x => x.Id > 0).ToArray(); // reject any detached

            this.Index(
                nodes.Where(x => x.ItemType == PublishedItemType.Content),
                indexerConfiguration.ShouldIndexContent,
                indexerConfiguration.ShouldIndexDetachedContent
            );

            this.Index(
                nodes.Where(x => x.ItemType == PublishedItemType.Media),
                indexerConfiguration.ShouldIndexMedia,
                indexerConfiguration.ShouldIndexDetachedMedia
            );

            this.Index(
                nodes.Where(x => x.ItemType == PublishedItemType.Member),
                indexerConfiguration.ShouldIndexMembers,
                indexerConfiguration.ShouldIndexDetachedMembers
            );
        }

        /// <summary>
        /// index all supplied nodes (and their detached content)
        /// </summary>
        /// <param name="nodes">collection of nodes conent/media/member nodes to be indexed</param>
        /// <param name="indexItem">when true, indicates the IPublishedContent nodes should be indexed</param>
        /// <param name="indexDetached">when true, indicates the detached items for each node should be indexed</param>
        internal void Index(IEnumerable<IPublishedContent> nodes, bool indexItem, bool indexDetached)
        {
            if (!indexItem && !indexDetached) return; // possible 

            var stopwatch = Stopwatch.StartNew();
            var counter = 0;

            var indexWriter = this.GetIndexWriter();

            foreach(var node in nodes)
            {
                IndexingContext indexingContext;
                Document document;

                if (indexItem)
                {
                    indexingContext = new IndexingContext(null, node, this.Name);

                    document = new Document();

                    LookService.Index(indexingContext, document);

                    if (!indexingContext.Cancelled)
                    {
                        counter++;

                        indexWriter.AddDocument(document);
                    }
                }

                if (indexDetached)
                {
                    IPublishedContent[] detachedNodes = null;

                    try 
                    {
                        // SEOChecker prior to 2.2 doesn't handle IPublishedContent without an ID
                        detachedNodes = node.GetDetachedDescendants();
                    }
                    catch (Exception exception)
                    {
                        LogHelper.WarnWithException(typeof(LookIndexer), "Error handling Detached items", exception);                       
                    }
                    finally
                    {
                        if (detachedNodes != null)
                        {
                            foreach (var detachedNode in detachedNodes)
                            {
                                indexingContext = new IndexingContext(node, detachedNode, this.Name);

                                document = new Document();

                                LookService.Index(indexingContext, document);

                                if (!indexingContext.Cancelled)
                                {
                                    counter++;

                                    indexWriter.AddDocument(document); // index each detached item
                                }
                            }
                        }
                    }
                }
            }

            indexWriter.Commit();

            stopwatch.Stop();

            if (counter > 0)
            {
                LogHelper.Debug(typeof(LookIndexer), $"Indexing { counter } Item(s) Took { stopwatch.ElapsedMilliseconds }ms");
            }
        }
    }
}
