namespace Our.Umbraco.Look.Models
{
    public class TagQuery
    {
        public Tag[] AllTags { get; set; }

        public Tag[] AnyTags { get; set; }

        public Tag[] NotTags { get; set; }

        /// <summary>
        /// when null, facets are not calculated, but when string[], each string value represents the tag group field to facet on, the empty string or whitespace = empty group
        /// The count value for a returned tag indicates how may results would be expected should that tag be added into the AllTags collection of this query
        /// </summary>
        public string[] GetFacets { get; set; } = null;
    }
}
