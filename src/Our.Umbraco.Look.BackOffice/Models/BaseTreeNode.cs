using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal abstract class BaseTreeNode : ILookTreeNode
    {
        public string Id { get; }

        public FormDataCollection QueryStrings { get; }

        public abstract string Name { get; }

        public abstract string Icon { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="queryStrings"></param>
        protected BaseTreeNode(string id, FormDataCollection queryStrings)
        {
            this.Id = id;
            this.QueryStrings = queryStrings;
        }

        /// <summary>
        /// Default to an empty collection (no children)
        /// </summary>
        public virtual ILookTreeNode[] GetChildren()
        {
            return new ILookTreeNode[] { };
        }
    }
}
