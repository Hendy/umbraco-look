using System;
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
        public ItemType[] ItemTypes { internal get; set; } = new[] {
                                                                ItemType.Content,
                                                                ItemType.DetachedContent,
                                                                ItemType.Media,
                                                                ItemType.DetachedMedia,
                                                                ItemType.Member,
                                                                ItemType.DetachedMember };

        /// <summary>
        /// null = no filtering, otherwize only index items with a docType, mediaType or memberType in this array
        /// </summary>
        public string[] Aliases { internal get; set; }

        public Action<IndexingContext> BeforeIndexing { internal get; set; }

        public Func<IndexingContext, string> NameIndexer { internal get; set; }

        public Func<IndexingContext, DateTime?> DateIndexer { internal get; set; }

        public Func<IndexingContext, string> TextIndexer { internal get; set; }

        public Func<IndexingContext, LookTag[]> TagIndexer { internal get; set; }

        public Func<IndexingContext, Location> LocationIndexer { internal get; set; }

        public Action<IndexingContext> AfterIndexing { internal get; set; }

        /// <summary>
        /// Helper to indicate whether content should be indexed
        /// </summary>
        internal bool ShouldIndexContent => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Content);

        /// <summary>
        /// Helper to indicate whether detached items on content should be indexed
        /// </summary>
        internal bool ShouldIndexDetachedContent => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedContent);

        /// <summary>
        /// Helper to indicate whether media should be indexed
        /// </summary>
        internal bool ShouldIndexMedia => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Media);

        /// <summary>
        /// Helper to indicate whether detached items on media should be indexed
        /// </summary>
        internal bool ShouldIndexDetachedMedia => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.DetachedMedia);

        /// <summary>
        /// Helper to indicate whether members should be indexed
        /// </summary>
        internal bool ShouldIndexMembers => this.ItemTypes != null && this.ItemTypes.Contains(ItemType.Member);

        /// <summary>
        /// Helper to indicate whether detached items on content members be indexed
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
