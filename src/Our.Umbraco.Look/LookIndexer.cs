using Examine;
using Examine.LuceneEngine;
using Examine.LuceneEngine.Config;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace Our.Umbraco.Look
{
    public class DetachedIndexer : LuceneIndexer
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }



        protected override void PerformIndexAll(string type)
        {
        }

        /// <summary>
        /// triggered from a back office rebuild 
        /// </summary>
        protected override void PerformIndexRebuild()
        {

            this.OnIndexOperationComplete(new EventArgs()); // causes the backoffice rebuild to end (UI thread watches a cache value)
        }

        protected override void AddDocument(Dictionary<string, string> fields, IndexWriter writer, int nodeId, string type)
        {
            base.AddDocument(fields, writer, nodeId, type);
        }
        protected override void AddSingleNodeToIndex(XElement node, string type)
        {
            base.AddSingleNodeToIndex(node, type);
        }
        protected override IndexWriter CreateIndexWriter()
        {
            return base.CreateIndexWriter();
        }
        public override void DeleteFromIndex(string nodeId)
        {
            base.DeleteFromIndex(nodeId);
        }
        protected override Dictionary<string, string> GetDataToIndex(XElement node, string type)
        {
            return base.GetDataToIndex(node, type);
        }
        protected override IIndexCriteria GetIndexerData(IndexSet indexSet)
        {
            return base.GetIndexerData(indexSet);
        }
        public override Directory GetLuceneDirectory()
        {
            return base.GetLuceneDirectory();
        }
        protected override FieldIndexTypes GetPolicy(string fieldName)
        {
            return base.GetPolicy(fieldName);
        }
        protected override Dictionary<string, string> GetSpecialFieldsToIndex(Dictionary<string, string> allValuesForIndexing)
        {
            return base.GetSpecialFieldsToIndex(allValuesForIndexing);
        }
        public override void IndexAll(string type)
        {
            base.IndexAll(type);
        }
        public override bool IndexExists()
        {
            return base.IndexExists();
        }
        protected override void OnDocumentWriting(DocumentWritingEventArgs docArgs)
        {
            base.OnDocumentWriting(docArgs);
        }
        protected override void OnDuplicateFieldWarning(int nodeId, string indexSetName, string fieldName)
        {
            base.OnDuplicateFieldWarning(nodeId, indexSetName, fieldName);
        }
        protected override void OnGatheringFieldData(IndexingFieldDataEventArgs e)
        {
            base.OnGatheringFieldData(e);
        }
        protected override void OnGatheringNodeData(IndexingNodeDataEventArgs e)
        {
            base.OnGatheringNodeData(e);
        }
        protected override void OnIgnoringNode(IndexingNodeDataEventArgs e)
        {
            base.OnIgnoringNode(e);
        }
        protected override void OnIndexDeleted(DeleteIndexEventArgs e)
        {
            base.OnIndexDeleted(e);
        }
        protected override void OnIndexingError(IndexingErrorEventArgs e)
        {
            base.OnIndexingError(e);
        }
        protected override void OnIndexOperationComplete(EventArgs e)
        {
            base.OnIndexOperationComplete(e);
        }
        protected override void OnIndexOptimized(EventArgs e)
        {
            base.OnIndexOptimized(e);
        }
        protected override void OnIndexOptimizing(EventArgs e)
        {
            base.OnIndexOptimizing(e);
        }
        protected override void OnNodeIndexed(IndexedNodeEventArgs e)
        {
            base.OnNodeIndexed(e);
        }
        protected override void OnNodeIndexing(IndexingNodeEventArgs e)
        {
            base.OnNodeIndexing(e);
        }
        protected override void OnNodesIndexed(IndexedNodesEventArgs e)
        {
            base.OnNodesIndexed(e);
        }
        protected override void OnNodesIndexing(IndexingNodesEventArgs e)
        {
            base.OnNodesIndexing(e);
        }
        public override void RebuildIndex()
        {
            base.RebuildIndex();
        }
        public override void ReIndexNode(XElement node, string type)
        {
            base.ReIndexNode(node, type);
        }
        protected override bool ValidateDocument(XElement node)
        {
            return base.ValidateDocument(node);
        }
    }
}
