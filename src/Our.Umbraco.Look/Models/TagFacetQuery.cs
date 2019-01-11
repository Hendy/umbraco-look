using Our.Umbraco.Look.Extensions;

namespace Our.Umbraco.Look
{
    public class TagFacetQuery
    {
        public string[] TagGroups { get; set; }

        /// <summary>
        /// New facet query for tags in the default un-named group
        /// </summary>
        public TagFacetQuery()
        {
            this.TagGroups = new string[] { "" };
        }

        /// <summary>
        /// Create a new facet query for tags in groups
        /// </summary>
        /// <param name="tagGroups">All tags in the supplied tag groups will be returned with facet counts</param>
        public TagFacetQuery(params string[] tagGroups)
        {
            this.TagGroups = tagGroups;
        }

        public override bool Equals(object obj)
        {
            var tagFacetQuery = obj as TagFacetQuery;

            return tagFacetQuery != null
                    && ((tagFacetQuery.TagGroups == null && this.TagGroups == null) || (tagFacetQuery.TagGroups != null && this.TagGroups != null && tagFacetQuery.TagGroups.BothNullOrElementsEqual(this.TagGroups)));
        }
    }
}
