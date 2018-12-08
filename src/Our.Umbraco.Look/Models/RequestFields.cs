namespace Our.Umbraco.Look
{
    /// <summary>
    /// Enum to specify which fields should be returned from Lucene
    /// </summary>
    public enum RequestFields
    {
        /// <summary>
        /// Optimised to return only the Lucene fields required for the item to populate the LookMatch obj
        /// </summary>
        LookFieldsOnly,

        /// <summary>
        /// Return all Lucene fields associated with the item, this enables the SearchResult.Fields property to be 
        /// fully populated (as Examine does)
        /// </summary>
        AllFields
    }
}
