namespace Our.Umbraco.Look
{
    public enum DetachedQuery
    {
        /// <summary>
        /// No filtering - detached items will be returned if they match
        /// </summary>
        IncludeDetached,

        /// <summary>
        /// Detached item will not be returned
        /// </summary>
        ExcludeDetached,

        /// <summary>
        /// Only detached items will be returned
        /// </summary>
        OnlyDetached
    }
}
