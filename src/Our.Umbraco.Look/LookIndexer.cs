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
        UmbracoHelper _umbracoHelper = null;

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

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        /// <summary>
        /// Triggered (via base.RebuildIndex) when for Umbraco back office rebuild event
        /// </summary>
        protected override void PerformIndexRebuild()
        {
            // attmept to get indexer configuration, if not specified, then fallback to assuming all content, media, members and detached items should be handled
            var indexerConfiguration = LookConfiguration.IndexerConfiguration[this.Name] ?? IndexerConfiguration.GetDefaultIndexerConfiguration();
                                                                                            
            if (indexerConfiguration.IndexContent || indexerConfiguration.IndexContentDetached)
            {
                var content = this.UmbracoHelper.TypedContentAtXPath("//*[@isDoc]");

                this.Index(content, indexerConfiguration.IndexContentDetached);
            }

            if (indexerConfiguration.IndexMedia || indexerConfiguration.IndexMediaDetached)
            {
                var media = this.UmbracoHelper.TypedMediaAtRoot().SelectMany(x => x.DescendantsOrSelf());

                this.Index(media, indexerConfiguration.IndexMediaDetached);
            }

            if (indexerConfiguration.IndexMembers || indexerConfiguration.IndexMembersDetached)
            {
                var members = ApplicationContext.Current.Services.MemberService.GetAll(0, int.MaxValue, out int totalRecords).Select(x => this.UmbracoHelper.TypedMember(x.Id));

                this.Index(members, indexerConfiguration.IndexMembersDetached);
            }

            this.GetIndexWriter().Optimize();

            this.OnIndexOperationComplete(new EventArgs()); // causes the backoffice rebuild to end (UI thread watches a cache value)
        }

        protected override void PerformIndexAll(string type)
        {
        }

        /// <summary>
        /// index all supplied nodes (and their detached content)
        /// </summary>
        /// <param name="nodes">collection of nodes conent/media/member nodes to be indexed</param>
        /// <param name="indexDetached">flag to indicate whether detached items (for each of the supplied nodes) should atttemp to be indexed</param>
        internal void Index(IEnumerable<IPublishedContent> nodes, bool indexDetached)
        {
            var stopwatch = Stopwatch.StartNew();
            var counter = 0;

            var indexWriter = this.GetIndexWriter();

            foreach(var node in nodes)
            {
                var indexingContext = new IndexingContext(
                                                hostNode: null,
                                                node: node,
                                                indexerName: this.Name);

                var document = new Document();

                LookService.Index(indexingContext, document);

                if (!indexingContext.Cancelled)
                {
                    counter++;

                    indexWriter.AddDocument(document);
                }

                if (indexDetached)
                {
                    try // SEOChecker prior to 2.2 doesn't handle IPublishedContent without an ID
                    {
                        foreach (var detachedNode in node.GetDetachedDescendants())
                        {
                            indexingContext = new IndexingContext(
                                                    hostNode: node,
                                                    node: detachedNode,
                                                    indexerName: this.Name);

                            document = new Document();

                            LookService.Index(indexingContext, document);

                            if (!indexingContext.Cancelled)
                            {
                                counter++;

                                indexWriter.AddDocument(document); // index each detached item
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        LogHelper.WarnWithException(typeof(LookIndexer), "Error handling Detached items", exception);                       
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
