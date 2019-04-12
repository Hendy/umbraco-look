using System.Linq;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Configuration behaviour for a given indexer
    /// </summary>
    public class IndexerConfiguration
    {
        /// <summary>
        /// Set item types to include in the index, defaults to all item types
        /// </summary>
        public ItemType[] ItemTypes { get; set; } = new[] {
                                                    ItemType.Content,
                                                    ItemType.DetachedContent,
                                                    ItemType.Media,
                                                    ItemType.DetachedMedia,
                                                    ItemType.Member,
                                                    ItemType.DetachedMember };

        ///// <summary>
        ///// 
        ///// </summary>
        //public string[] Aliases { get; set; }

        //public IndexerType[] Indexers = new [] { IndexerType.Name, IndexerType.Date, IndexerType.Text, IndexerType.Tag, IndexerType.Location  }

        /// <summary>
        /// Flag to indicate whether content should be indexed
        /// </summary>
        internal bool IndexContent => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Content);

        /// <summary>
        /// Flag to indicate whether detached items on content should be indexed
        /// </summary>
        internal bool IndexDetachedContent => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedContent);

        /// <summary>
        /// Flag to indicate whether media should be indexed
        /// </summary>
        internal bool IndexMedia => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Media);

        /// <summary>
        /// Flag to indicate whether detached items on media should be indexed
        /// </summary>
        internal bool IndexDetachedMedia => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedMedia);

        /// <summary>
        /// Flag to indicate whether members should be indexed
        /// </summary>
        internal bool IndexMembers => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Member);

        /// <summary>
        /// Flag to indicate whether detached items on content members be indexed
        /// </summary>
        internal bool IndexDetachedMembers => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedMember);
    }
}
