namespace Our.Umbraco.Look
{
    /// <summary>
    /// Enum options each representing a type of IPublishedContent
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Represents PublishedItemType.Content
        /// </summary>
        Content,

        /// <summary>
        /// Respresents a detached item on PublishedItemType.Content
        /// </summary>
        DetachedContent,

        /// <summary>
        /// Represents PublishedItemType.Media
        /// </summary>
        Media,

        /// <summary>
        /// Respresents a detached item on PublishedItemType.Media
        /// </summary>
        DetachedMedia,

        /// <summary>
        /// Represents PublishedItemType.Member
        /// </summary>
        Member,

        /// <summary>
        /// Respresents a detached item on PublishedItemType.Member
        /// </summary>
        DetachedMember
    }
}
