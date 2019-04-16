using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Index;
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
        /// <summary>
        /// Get the configuration model for this indexer
        /// </summary>
        private IndexerConfiguration Configuration => LookService.GetIndexerConfiguration(this.Name); // multiple gets shouldn't be too slow

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        /// <summary>
        /// Triggered (via base.RebuildIndex) when for Umbraco back office rebuild event
        /// </summary>
        protected override void PerformIndexRebuild()
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            if (this.Configuration.ShouldIndexContent || this.Configuration.ShouldIndexDetachedContent)
            {
                var content = umbracoHelper.TypedContentAtXPath("//*[@isDoc]");

                this.Index(
                        content,
                        this.Configuration.ShouldIndexContent,
                        this.Configuration.ShouldIndexDetachedContent);
            }

            if (this.Configuration.ShouldIndexMedia || this.Configuration.ShouldIndexDetachedMedia)
            {
                var media = umbracoHelper
                                .TypedMediaAtRoot()
                                .SelectMany(x => x.DescendantsOrSelf());

                this.Index(
                        media,
                        this.Configuration.ShouldIndexMedia,
                        this.Configuration.ShouldIndexDetachedMedia);
            }

            if (this.Configuration.ShouldIndexMembers || this.Configuration.ShouldIndexDetachedMembers)
            {
                var members = ApplicationContext
                                .Current
                                .Services
                                .MemberService
                                .GetAll(0, int.MaxValue, out int totalRecords)
                                .Select(x => umbracoHelper.TypedMember(x.Id));

                this.Index(
                        members,
                        this.Configuration.ShouldIndexMembers,
                        this.Configuration.ShouldIndexDetachedMembers);
            }

            this.GetIndexWriter().Optimize();

            this.OnIndexOperationComplete(new EventArgs()); // causes the backoffice rebuild to end (UI thread watches a cache value)
        }

        protected override void PerformIndexAll(string type)
        {
        }

        /// <summary>
        /// Index all Umbraco nodes with the supplied Ids (can be content, media, members, or a mixture)
        /// </summary>
        /// <param name="ids"></param>
        public void ReIndex(IEnumerable<int> ids)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            var nodes = ids
                        .Select(x => umbracoHelper.GetIPublishedContent(x))
                        .Where(x => x != null);

            this.ReIndex(nodes);
        }

        /// <summary>
        /// Index the supplied nodes (can be content, media, members, or a mixture)
        /// </summary>
        /// <param name="nodes"></param>
        public void ReIndex(IEnumerable<IPublishedContent> nodes)
        {
            nodes = nodes.Where(x => x.Id > 0).ToArray(); // reject any detached & enumerate into array

            // remove all first
            this.Remove(nodes.Select(x => x.Id).ToArray());

            this.Index(
                nodes.Where(x => x.ItemType == PublishedItemType.Content),
                this.Configuration.ShouldIndexContent,
                this.Configuration.ShouldIndexDetachedContent
            );

            this.Index(
                nodes.Where(x => x.ItemType == PublishedItemType.Media),
                this.Configuration.ShouldIndexMedia,
                this.Configuration.ShouldIndexDetachedMedia
            );

            this.Index(
                nodes.Where(x => x.ItemType == PublishedItemType.Member),
                this.Configuration.ShouldIndexMembers,
                this.Configuration.ShouldIndexDetachedMembers
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
            if (!nodes.Any()) return;

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

        /// <summary>
        /// remove all items & detached items for the Umbraco content, media or member ids
        /// </summary>
        /// <param name="ids"></param>
        internal void Remove(int[] ids)
        {
            var indexWriter = this.GetIndexWriter();

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
