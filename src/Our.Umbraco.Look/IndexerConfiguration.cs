namespace Our.Umbraco.Look
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexerConfiguration
    {
        /// <summary>
        /// Flag to indicate whether content should be indexed
        /// </summary>
        public bool IndexContent { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether detached items on content should be indexed
        /// </summary>
        public bool IndexContentDetached { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether media should be indexed
        /// </summary>
        public bool IndexMedia { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether detached items on media should be indexed
        /// </summary>
        public bool IndexMediaDetached { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether members should be indexed
        /// </summary>
        public bool IndexMembers { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether detached items on content members be indexed
        /// </summary>
        public bool IndexMembersDetached { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static IndexerConfiguration GetDefaultIndexerConfiguration()
        {
            return new IndexerConfiguration()
            {
                IndexContent = true,
                IndexContentDetached = true,
                IndexMedia = true,
                IndexMediaDetached = true,
                IndexMembers = true,
                IndexMembersDetached = true
            };
        }
    }
}
