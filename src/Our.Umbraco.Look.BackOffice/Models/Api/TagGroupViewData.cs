namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    /// <summary>
    /// Model for view when a TagGroup is selected in the tree
    /// </summary>
    public class TagGroupViewData
    {
        /// <summary>
        /// For all tags in a given tag group, return each with a useage count
        /// </summary>
        public TagCount[] TagCounts { get; set; }

        public class TagCount
        {
            public string Name { get; set; }

            public int Count { get; set; }
        }
    }
}
