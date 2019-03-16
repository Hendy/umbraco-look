using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Our.Umbraco.Look.Extensions;
using Our.Umbraco.Look.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
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
            var nodes = new List<IPublishedContent>();

            nodes.AddRange(this.UmbracoHelper.TypedContentAtXPath("//*[@isDoc]"));

            foreach(var media in this.UmbracoHelper.TypedMediaAtRoot())
            {
                nodes.AddRange(media.DescendantsOrSelf());
            }

            //foreach (var member in ApplicationContext.Current.Services.MemberService.GetAll(0, int.MaxValue, out int totalRecords))
            //{
            //    nodes.Add(this.UmbracoHelper.TypedMember(member.Id));
            //}

            this.Index(nodes.ToArray()); // index all nodes

            this.GetIndexWriter().Optimize();

            this.OnIndexOperationComplete(new EventArgs()); // causes the backoffice rebuild to end (UI thread watches a cache value)
        }

        protected override void PerformIndexAll(string type)
        {
        }

        //protected override void AddDocument(Dictionary<string, string> fields, IndexWriter writer, int nodeId, string type)
        //{
        //    base.AddDocument(fields, writer, nodeId, type);
        //}

        //protected override void AddSingleNodeToIndex(XElement node, string type)
        //{
        //    base.AddSingleNodeToIndex(node, type);
        //}

        //protected override IndexWriter CreateIndexWriter()
        //{
        //    var debug = base.CreateIndexWriter();
        //    return debug;
        //}

        //public override void DeleteFromIndex(string nodeId)
        //{
        //    base.DeleteFromIndex(nodeId);
        //}

        //protected override Dictionary<string, string> GetDataToIndex(XElement node, string type)
        //{
        //    var debug = base.GetDataToIndex(node, type);
        //    return debug;
        //}

        //protected override IIndexCriteria GetIndexerData(IndexSet indexSet)
        //{
        //    var debug = base.GetIndexerData(indexSet);
        //    return debug;
        //}

        //public override Directory GetLuceneDirectory()
        //{
        //    var debug = base.GetLuceneDirectory();
        //    return debug;
        //}

        //protected override FieldIndexTypes GetPolicy(string fieldName)
        //{
        //    var debug = base.GetPolicy(fieldName);
        //    return debug;
        //}

        //protected override Dictionary<string, string> GetSpecialFieldsToIndex(Dictionary<string, string> allValuesForIndexing)
        //{
        //    var debug = base.GetSpecialFieldsToIndex(allValuesForIndexing);
        //    return debug;
        //}

        //public override void IndexAll(string type)
        //{
        //    base.IndexAll(type);
        //}

        //public override bool IndexExists()
        //{
        //    var debug = base.IndexExists();
        //    return debug;
        //}

        //protected override void OnDocumentWriting(DocumentWritingEventArgs docArgs)
        //{
        //    base.OnDocumentWriting(docArgs);
        //}

        //protected override void OnDuplicateFieldWarning(int nodeId, string indexSetName, string fieldName)
        //{
        //    base.OnDuplicateFieldWarning(nodeId, indexSetName, fieldName);
        //}

        //protected override void OnGatheringFieldData(IndexingFieldDataEventArgs e)
        //{
        //    base.OnGatheringFieldData(e);
        //}

        //protected override void OnGatheringNodeData(IndexingNodeDataEventArgs e)
        //{
        //    base.OnGatheringNodeData(e);
        //}

        //protected override void OnIgnoringNode(IndexingNodeDataEventArgs e)
        //{
        //    base.OnIgnoringNode(e);
        //}

        //protected override void OnIndexDeleted(DeleteIndexEventArgs e)
        //{
        //    base.OnIndexDeleted(e);
        //}

        //protected override void OnIndexingError(IndexingErrorEventArgs e)
        //{
        //    base.OnIndexingError(e);
        //}

        //protected override void OnIndexOperationComplete(EventArgs e)
        //{
        //    base.OnIndexOperationComplete(e);
        //}

        //protected override void OnIndexOptimized(EventArgs e)
        //{
        //    base.OnIndexOptimized(e);
        //}

        //protected override void OnIndexOptimizing(EventArgs e)
        //{
        //    base.OnIndexOptimizing(e);
        //}

        //protected override void OnNodeIndexed(IndexedNodeEventArgs e)
        //{
        //    base.OnNodeIndexed(e);
        //}

        //protected override void OnNodeIndexing(IndexingNodeEventArgs e)
        //{
        //    base.OnNodeIndexing(e);
        //}

        //protected override void OnNodesIndexed(IndexedNodesEventArgs e)
        //{
        //    base.OnNodesIndexed(e);
        //}

        //protected override void OnNodesIndexing(IndexingNodesEventArgs e)
        //{
        //    base.OnNodesIndexing(e);
        //}

        //public override void ReIndexNode(XElement node, string type)
        //{
        //    base.ReIndexNode(node, type);
        //}

        //protected override bool ValidateDocument(XElement node)
        //{
        //    return base.ValidateDocument(node);
        //}

        /// <summary>
        /// index all supplied nodes (and their detached content)
        /// </summary>
        /// <param name="nodes">collection of nodes (and any detached content they may have) to be indexed</param>
        internal void Index(IPublishedContent[] nodes)
        {
            var indexWriter = this.GetIndexWriter();

            Parallel.ForEach(nodes, node =>
            {
                var indexingContext = new IndexingContext(
                                                hostNode: null,
                                                node: node,
                                                indexerName: this.Name);

                var document = new Document();

                LookService.Index(indexingContext, document);

                indexWriter.AddDocument(document);

                foreach (var detachedNode in node.GetFlatDetachedDescendants())
                {
                    indexingContext = new IndexingContext(
                                            hostNode: node,
                                            node: detachedNode,
                                            indexerName: this.Name);

                    document = new Document();

                    LookService.Index(indexingContext, document);

                    indexWriter.AddDocument(document); // index each detached item
                }

                indexWriter.Commit();
            });

            //indexWriter.Optimize();
        }
    }
}
