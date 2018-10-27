namespace Our.Umbraco.Look.Models
{
    public class TagQuery
    {
        /// <summary>
        /// When set, each search result must contain all of these tags -- TODO: rename to Required
        /// </summary>
        public string[] AllTags { get; set; } = null;

        /// <summary>
        /// When set, each search result must contain at least one of these tags
        /// </summary>
        public string[] AnyTags { get; set; } = null;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string[] NotTags { get; set; } = null;

        /// <summary>
        /// Flag to indicate whether facets should be calculated for tags
        /// The count value for a tag indicates how may results would be expected should that tag be added into the AllTags collection of this query
        /// </summary>
        public bool GetFacets { get; set; } = false;
    }
}
