using System.Linq;
using Our.Umbraco.Look.Extensions;

namespace Our.Umbraco.Look.Models
{
    public class TagQuery
    {
        //public Tag[] AllTags { get; set; }

        //public Tag[] AnyTags { get; set; }

        //public Tag[] NotTags { get; set; }

        /// <summary>
        /// Null or collection of tags known to be valid (as cleaned in the setter)
        /// </summary>
        private Tag[] _allTags;

        /// <summary>
        /// Null or collection of tags known to be valid (as cleaned in the setter)
        /// </summary>
        private Tag[] _anyTags;

        ///// <summary>
        ///// Null or collection of tags known to be valid (as cleaned in the setter)
        ///// </summary>
        //private Tag[] _notTags;

        /// <summary>
        /// When set, each search result must contain all of these tags
        /// </summary>
        public Tag[] AllTags
        {
            get
            {
                return this._allTags;
            }
            set
            {
                this._allTags = value?.Where(x => x.Name.IsValidTag()).ToArray();
            }
        }

        /// <summary>
        /// When set, each search result must contain at least one of these tags
        /// </summary>
        public Tag[] AnyTags
        {
            get
            {
                return this._anyTags;
            }
            set
            {
                this._anyTags = value?.Where(x => x.Name.IsValidTag()).ToArray();
            }
        }

        /////// <summary>
        /////// When set, each search result must not contain any of these tags
        /////// </summary>
        ////public Tag[] NotTags
        ////{
        ////    get
        ////    {
        ////        return this._notTags;
        ////    }
        ////    set
        ////    {
        ////        this._notTags = value?.Where(x => x.IsValidTag()).ToArray();
        ////    }
        ////}

        /// <summary>
        /// when null, facets are not calculated, but when string[], each string value represents the tag group field to facet on, the empty string or whitespace = empty group
        /// The count value for a returned tag indicates how may results would be expected should that tag be added into the AllTags collection of this query
        /// </summary>
        public string[] GetFacets { get; set; } = null;
    }
}
