using System.Linq;

namespace Our.Umbraco.Look.Models
{
    public class NodeQuery
    {
        /// <summary>
        /// Collection of Node type aliases that a results can be any one of (when empty = has no effect)
        /// </summary>
        public string[] TypeAliases { get; set; } = null; // TODO: rename to aliases ?

        /// <summary>
        /// Any umbraco ids that should be exlcuded from the results (performed as part of the Lucene query so as to get an accurate result count)
        /// </summary>
        public int[] NotIds { get; set; } = null;

        /// <summary>
        /// Create new empty NodeQuery search criteria
        /// </summary>
        public NodeQuery()
        {
        }

        /// <summary>
        /// Create new NodeQuery search criteria, by content, media or member alias
        /// </summary>
        /// <param name="typeAlias">the alias of the content, media or member</param>
        public NodeQuery(string typeAlias)
        {
            this.TypeAliases = new string[] { typeAlias };
        }

        //public NodeQuery(int notId)
        //{
        //    this.ExcludeIds = new int[] { notId };
        //}

        //public NodeQuery(string typeAlias, int notId)
        //{
        //    this.TypeAliases = new string[] { typeAlias };
        //    this.ExcludeIds = new int[] { notId };
        //}

        public override bool Equals(object obj)
        {
            NodeQuery nodeQuery = obj as NodeQuery;

            return nodeQuery != null
                && ((nodeQuery.TypeAliases == null && this.TypeAliases == null)
                    || (nodeQuery.TypeAliases != null && this.TypeAliases != null && nodeQuery.TypeAliases.SequenceEqual(this.TypeAliases)))
                && ((nodeQuery.NotIds == null && this.NotIds == null)
                    || (nodeQuery.NotIds != null && this.NotIds != null && nodeQuery.NotIds.SequenceEqual(this.NotIds)));
        }

        internal NodeQuery Clone()
        {
            return (NodeQuery)this.MemberwiseClone();
        }
    }
}
