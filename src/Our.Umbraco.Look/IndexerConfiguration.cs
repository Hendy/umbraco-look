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

        /// <summary>
        /// null = no filtering, otherwize only index items with a docType, mediaType or memberType in this array
        /// </summary>
        public string[] Aliases { get; set; } = null;

        // TODO: BeforeIndexing, NameIndexer, DateIndexer, TextIndexer, TagIndexer, LoccationIndexer, AfterIndexing

        /// <summary>
        /// Flag to indicate whether content should be indexed
        /// </summary>
        internal bool ShouldIndexContent => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Content);

        /// <summary>
        /// Flag to indicate whether detached items on content should be indexed
        /// </summary>
        internal bool ShouldIndexDetachedContent => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedContent);

        /// <summary>
        /// Flag to indicate whether media should be indexed
        /// </summary>
        internal bool ShouldIndexMedia => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Media);

        /// <summary>
        /// Flag to indicate whether detached items on media should be indexed
        /// </summary>
        internal bool ShouldIndexDetachedMedia => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedMedia);

        /// <summary>
        /// Flag to indicate whether members should be indexed
        /// </summary>
        internal bool ShouldIndexMembers => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Member);

        /// <summary>
        /// Flag to indicate whether detached items on content members be indexed
        /// </summary>
        internal bool ShouldIndexDetachedMembers => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedMember);

        /// <summary>
        /// Helper method to check to see if a given docType, mediaType or memberType alias should be indexed
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        internal bool ShouldIndexAlias(string alias) => this.Aliases == null || alias != null && this.Aliases.Contains(alias);
    }
}
