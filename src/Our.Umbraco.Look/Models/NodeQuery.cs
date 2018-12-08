using System.Globalization;
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
        /// The cultures of the content nodes to find
        /// </summary>
        public CultureInfo[] Cultures { get; set; } = null;

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
        /// Create new NodeQuery search critera (specifying a culture implies that the PublishedItemType will be Content)
        /// </summary>
        /// <param name="culture"></param>
        public NodeQuery(CultureInfo cultureInfo)
        {
            this.Cultures = new CultureInfo[] { cultureInfo };
        }

        /// <summary>
        /// Create new NodeQuery search criteria for all nodes of any of these aliases
        /// </summary>
        /// <param name="aliases">array of string aliases for the content, media or members</param>
        public NodeQuery(params string[] aliases)
        {
            this.Aliases = aliases;
        }

        /// <summary>
        /// Create new NodeQuery search critera 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="aliases"></param>
        public NodeQuery(PublishedItemType type, params string[] aliases)
        {
            this.Types = new PublishedItemType[] { type };
            this.Aliases = aliases;
        }

        /// <summary>
        /// Create new NodeQuery search critera 
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="aliases"></param>
        public NodeQuery(CultureInfo cultureInfo, params string[] aliases)
        {
            this.Cultures = new CultureInfo[] { cultureInfo };
            this.Aliases = aliases;
        }

        public override bool Equals(object obj)
        {
            NodeQuery nodeQuery = obj as NodeQuery;

            return nodeQuery != null
                && ((nodeQuery.Types == null && this.Types == null)
                    || (nodeQuery.Types != null && this.Types != null && nodeQuery.Types.SequenceEqual(this.Types)))
                && ((nodeQuery.Cultures == null && this.Cultures == null)
                    || (nodeQuery.Cultures != null && this.Cultures != null && nodeQuery.Cultures.SequenceEqual(this.Cultures)))
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
