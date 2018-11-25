using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Look.Models
{
    public class TagQuery
    {
        public LookTag[] AllTags { get; set; }

        public LookTag[] AnyTags { get; set; }

        public LookTag[] NotTags { get; set; }

        /// <summary>
        /// when null, facets are not calculated, but when string[], each string value represents the tag group field to facet on, the empty string or whitespace = empty group
        /// The count value for a returned tag indicates how may results would be expected should that tag be added into the AllTags collection of this query
        /// </summary>
        public string[] GetFacets { get; set; } = null;

        /// <summary>
        /// Helper to simplify the construction of LookTag array, by being able to supply a raw collection of tag strings
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static LookTag[] MakeTags(params string[] tags)
        {
            List<LookTag> lookTags = new List<LookTag>();

            if (tags != null)
            {
                foreach(var tag in tags)
                {
                    lookTags.Add(new LookTag(tag));
                }
            }

            return lookTags.ToArray();
        }

        public override bool Equals(object obj)
        {
            var tagQuery = obj as TagQuery;

            return tagQuery != null
                && ((tagQuery.AllTags == null && this.AllTags == null)
                    || (tagQuery.AllTags != null && this.AllTags != null && tagQuery.AllTags.SequenceEqual(this.AllTags)))
                && ((tagQuery.AnyTags == null && this.AnyTags == null)
                    || (tagQuery.AnyTags != null && this.AnyTags != null && tagQuery.AnyTags.SequenceEqual(this.AnyTags)))
                && ((tagQuery.NotTags == null && this.NotTags == null)
                    || (tagQuery.NotTags != null && this.NotTags != null && tagQuery.NotTags.SequenceEqual(this.NotTags)))
                && tagQuery.GetFacets == this.GetFacets;
        }

        internal TagQuery Clone()
        {
            return (TagQuery)this.MemberwiseClone();
        }
    }
}
