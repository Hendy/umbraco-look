using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal abstract class BaseTreeNode : ILookTreeNode
    {
        public string Id { get; }

        public FormDataCollection QueryStrings { get; }

        public abstract string Name { get; }

        public abstract string Icon { get; }

        public virtual string RoutePath { get; } = "developer";

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

        /// <summary>
        /// default to empty collection
        /// </summary>
        /// <returns></returns>
        public virtual MenuItemCollection GetMenu()
        {
            return new MenuItemCollection();
        }
    }
}
