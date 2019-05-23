using Our.Umbraco.Look.Extensions;
using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Model passed into any custom consumer indexing methods - supplies the data, as to what's being indexed, and to where it's being indexed
    /// </summary>
    public class IndexingContext
    {
        /// <summary>
        /// The name of the Examine indexer into which this Item is being indexed
        /// </summary>
        public string IndexerName { get; }

        /// <summary>
        /// When the item being indexed is 'detached', this is the IPublishedContent of the 'known' Content, Media or Member.
        /// (this value will null when the item being indexed is not detached)
        /// </summary>
        public IPublishedContent HostItem { get; }

        /// <summary>
        /// The Content, Media, Member or Detached item being indexed (always has a value (unless unit testing))
        /// </summary>
        public IPublishedContent Item { get; }

        /// <summary>
        /// The ItemType enum for the item being indexed.
        ///     Content
        ///     DetachedContent
        ///     Media
        ///     DetachedMedia
        ///     Member
        ///     DetachedMember
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// Convienience flag to indicate whether the item is a detached item
        /// </summary>
        [Obsolete("use ItemType.IsDetached() extension method instead")]
        public bool IsDetached => this.ItemType.IsDetached();

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
            this.IndexerName = indexerName;
            this.HostItem = hostNode;
            this.Item = node;

            if (hostNode != null) // detached
            {
                switch (hostNode.ItemType)
                {
                    case PublishedItemType.Content: this.ItemType = ItemType.DetachedContent; break;
                    case PublishedItemType.Media: this.ItemType = ItemType.DetachedMedia; break;
                    case PublishedItemType.Member: this.ItemType = ItemType.DetachedMember; break;
                }
            }
            else if (node != null) // not detached
            {
                switch (node.ItemType)
                {
                    case PublishedItemType.Content: this.ItemType = ItemType.Content; break;
                    case PublishedItemType.Media: this.ItemType = ItemType.Media; break;
                    case PublishedItemType.Member: this.ItemType = ItemType.Member; break;
                }
            }
        }

        /// <summary>
        /// When called, the indexing of the current item will be cancelled.
        /// If using an Exmaine indexer, then Look will stop adding data from the point of cancellation.
        /// If using a Look indexer, then full cancellation occurs and a Lucene document will not be created.
        /// </summary>
        public void Cancel()
        {
            this.Cancelled = true;
        }
    }
}
