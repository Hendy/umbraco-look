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
        public bool IndexContent { get; set; }

        /// <summary>
        /// Flag to indicate whether media should be indexed
        /// </summary>
        public bool IndexMedia { get; set; }

        /// <summary>
        /// Flag to indicate whether members should be indexed
        /// </summary>
        public bool IndexMembers { get; set; }

        /// <summary>
        /// Flag to indicate whether detached items on content should be indexed
        /// </summary>
        public bool IndexDetachedContent { get; set; }

        /// <summary>
        /// Flag to indicate whether detached items on media should be indexed
        /// </summary>
        public bool IndexDetachedMedia { get; set; }

        /// <summary>
        /// Flag to indicate whether detached items on content members be indexed
        /// </summary>
        public bool IndexDetachedMembers { get; set; }

        /// <summary>
        /// Constructor
        /// When an IndexerConfiguration is set, all features are disabled by default - set properties to enable
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
        /// Constructor
        /// Internal use for when IndexerConfiguration is not net, all features are enabled by default
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
