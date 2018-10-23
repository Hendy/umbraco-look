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
    }
}
