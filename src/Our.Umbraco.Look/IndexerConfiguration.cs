namespace Our.Umbraco.Look
{
    /// <summary>
    /// Configuration behaviour for a given indexer
    /// </summary>
    public class IndexerConfiguration
    {
        /// <summary>
        /// Flag to indicate whether content should be indexed
        /// </summary>
        public bool IndexContent { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether media should be indexed
        /// </summary>
        public bool IndexMedia { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether members should be indexed
        /// </summary>
        public bool IndexMembers { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether detached items on content should be indexed
        /// </summary>
        public bool IndexDetachedContent { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether detached items on media should be indexed
        /// </summary>
        public bool IndexDetachedMedia { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether detached items on content members be indexed
        /// </summary>
        public bool IndexDetachedMembers { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public IndexerConfiguration()
        {
            this.IndexContent = false;
            this.IndexMedia = false;
            this.IndexMembers = false;
            this.IndexDetachedContent = false;
            this.IndexDetachedMedia = false;
            this.IndexDetachedMembers = false;
        }

        /// <summary>
        /// Constructor (internal use for when consumer has not set an indexer configuration)
        /// </summary>
        /// <param name="notSet"></param>
        internal IndexerConfiguration(bool notSet)
        {
            this.IndexContent = true;
            this.IndexMedia = true;
            this.IndexMembers = true;
            this.IndexDetachedContent = true;
            this.IndexDetachedMedia = true;
            this.IndexDetachedMembers = true;
        }
    }
}
