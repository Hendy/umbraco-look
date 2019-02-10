using System.Collections.Generic;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    /// <summary>
    /// Model for view when a TagGroup is selected in the tree
    /// </summary>
    public class TagGroupViewData
    {
        /// <summary>
        /// Tags in this group - together with their useage count
        /// </summary>
        public Dictionary<LookTag, int> Tags { get; set; }
    }
}
