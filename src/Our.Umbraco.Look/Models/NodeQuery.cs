using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Models
{
    public class NodeQuery
    {
        /// <summary>
        /// The types of node to find, eg. Content, Media, Members
        /// </summary>
        public PublishedItemType[] Types { get; set; } = null;

        /// <summary>
        /// The document type, media type or member type aliases
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

        /// <summary>
        /// Create new NodeQuery search criteria for nodes of a given type, eg content, media or members
        /// </summary>
        /// <param name="type">The node type. eg. content, media or member</param>
        public NodeQuery(PublishedItemType type)
        {
            this.Types = new PublishedItemType[] { type };
        }

        /// <summary>
        /// Create new NodeQuery search criteria for nodes of a given alias
        /// </summary>
        /// <param name="alias">the alias of the content, media or member</param>
        public NodeQuery(string alias)
        {
            this.Aliases = new string[] { alias };
        }

        /// <summary>
        /// Create new NodeQuery search critera for nodes of a given type and alias
        /// </summary>
        /// <param name="nodeType">The node type. eg. content, media or member</param>
        /// <param name="alias">the alias of the content, media or member</param>
        public NodeQuery(PublishedItemType type, string alias)
        {
            this.Types = new PublishedItemType[] { type };
            this.Aliases = new string[] { alias };
        }

        // TODO: overloads for not id(s) ?

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
