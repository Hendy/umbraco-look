using System.Linq;

namespace Our.Umbraco.Look.Models
{
    public class NodeQuery
    {
        /// <summary>
        /// The types of node to find, eg. Content, Media, Members
        /// </summary>
        public NodeType[] Types { get; set; } = null;

        /// <summary>
        /// Collection of Node type aliases that a results can be any one of (when empty = has no effect)
        /// </summary>
        public string[] Aliases { get; set; } = null;

        /// <summary>
        /// Any umbraco ids that should be exlcuded from the results
        /// </summary>
        public int[] NotIds { get; set; } = null;

        /// <summary>
        /// Create new empty NodeQuery search criteria
        /// </summary>
        public NodeQuery()
        {
        }

        ///// <summary>
        ///// Create new NodeQuery search criteria for nodes of a given type, eg content, media or members
        ///// </summary>
        ///// <param name="nodeType"></param>
        //public NodeQuery(NodeType nodeType)
        //{
        //    this.NodeTypes = new NodeType[] { nodeType };
        //}

        /// <summary>
        /// Create new NodeQuery search criteria, by content, media or member alias
        /// </summary>
        /// <param name="typeAlias">the alias of the content, media or member</param>
        public NodeQuery(string typeAlias)
        {
            this.Aliases = new string[] { typeAlias };
        }

        //public NodeQuery(NodeType nodeType, string alias)
        //{

        //}

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
                && ((nodeQuery.Aliases == null && this.Aliases == null)
                    || (nodeQuery.Aliases != null && this.Aliases != null && nodeQuery.Aliases.SequenceEqual(this.Aliases)))
                && ((nodeQuery.NotIds == null && this.NotIds == null)
                    || (nodeQuery.NotIds != null && this.NotIds != null && nodeQuery.NotIds.SequenceEqual(this.NotIds)));
        }

        internal NodeQuery Clone()
        {
            return (NodeQuery)this.MemberwiseClone();
        }
    }
}
