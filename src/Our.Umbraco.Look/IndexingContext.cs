using Umbraco.Core.Models;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Model passed into any custom consumer indexing methods - supplies the data, as to what's being indexed, and to where it's being indexed
    /// </summary>
    public class IndexingContext
    {
        /// <summary>
        /// When detached content is being indexed, this property will be the IPublishedContent of the content, media or member containing the detached item being indexed
        /// </summary>
        public IPublishedContent HostItem { get; }

        /// <summary>
        /// The IPublishedContent representation of the Content, Media, Member or Detached item being indexed
        /// </summary>
        public IPublishedContent Item { get; }

        /// <summary>
        /// Flag to indicate whether the Item is a detached item
        /// </summary>
        public bool IsDetached => this.HostItem != null;

        /// <summary>
        /// The name of the Examine indexer into which this item is being indexed
        /// </summary>
        public string IndexerName { get; }

        /// <summary>
        /// Returns true if the Cancel method was called
        /// </summary>
        internal bool Cancelled { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostNode">Only set when detached content ins being indexed</param>
        /// <param name="node">The IPublishedContent representation of the thing being indexed (content, media, member or detached)</param>
        /// <param name="indexerName">The name of the inder being used</param>
        internal IndexingContext(IPublishedContent hostNode, IPublishedContent node, string indexerName)
        {
            this.HostItem = hostNode;
            this.Item = node;
            this.IndexerName = indexerName;
        }

        /// <summary>
        /// When called, the current IPublishedContent item will not be indexed.
        /// If using an Examine Umbraco indexer, then Look will not add any data to the Lucene document being indexed.
        /// If using a Look Exmaine indexer, then Look will not create a Lucene document for the IPublishedContent item being indexed.
        /// </summary>
        public void Cancel()
        {
            this.Cancelled = true;
        }
    }
}
