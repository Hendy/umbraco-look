using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Model passed into any custom cuonsumer indexing methods - supplies the data, as to what's being indexed, and to where it's being indexed
    /// </summary>
    public class IndexingContext
    {
        /// <summary>
        /// The IPublishedContent representation of the Content, Media or Member being indexed
        /// </summary>
        public IPublishedContent Node { get; }

        /// <summary>
        /// Enum to indicate whether the IPublishedContent is Content, Media or Member
        /// </summary>
        public NodeType NodeType { get; }

        /// <summary>
        /// The name of the Examine indexer into which this item is being indexed
        /// </summary>
        public string IndexerName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeType"></param>
        /// <param name="indexerName"></param>
        internal IndexingContext(IPublishedContent node, NodeType nodeType, string indexerName)
        {
            this.Node = node;
            this.NodeType = nodeType;
            this.IndexerName = indexerName;
        }
    }
}
