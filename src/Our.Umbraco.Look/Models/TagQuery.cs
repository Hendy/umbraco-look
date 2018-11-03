using System.Linq;
using Our.Umbraco.Look.Extensions;

namespace Our.Umbraco.Look.Models
{
    public class TagQuery
    {
        /// <summary>
        /// Null or collection of tags known to be valid (as cleaned in the setter)
        /// </summary>
        private string[] _allTags;

        /// <summary>
        /// Null or collection of tags known to be valid (as cleaned in the setter)
        /// </summary>
        private string[] _anyTags;

        ///// <summary>
        ///// Null or collection of tags known to be valid (as cleaned in the setter)
        ///// </summary>
        //private string[] _notTags;

        /// <summary>
        /// When set, each search result must contain all of these tags
        /// </summary>
        public string[] AllTags
        {
            get
            {
                return this._allTags;
            }
            set
            {
                this._allTags = value?.Where(x => x.IsValidTag()).ToArray();
            }
        }

        /// <summary>
        /// When set, each search result must contain at least one of these tags
        /// </summary>
        public string[] AnyTags
        {
            get
            {
                return this._anyTags;
            }
            set
            {
                this._anyTags = value?.Where(x => x.IsValidTag()).ToArray();
            }
        }

        ///// <summary>
        ///// When set, each search result must not contain any of these tags
        ///// </summary>
        //public string[] NotTags
        //{
        //    get
        //    {
        //        return this._notTags;
        //    }
        //    set
        //    {
        //        this._notTags = value?.Where(x => x.IsValidTag()).ToArray();
        //    }
        //}

        /// <summary>
        /// Flag to indicate whether facets should be calculated for tags
        /// The count value for a tag indicates how may results would be expected should that tag be added into the AllTags collection of this query
        /// </summary>
        public bool GetFacets { get; set; } = false;
    }
}
