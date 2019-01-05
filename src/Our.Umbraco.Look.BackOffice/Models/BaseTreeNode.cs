using Our.Umbraco.Look.BackOffice.Interfaces;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal abstract class BaseTreeNode : ILookTreeNode
    {
        public string Id { get; }

        public abstract string Name { get; }

        public abstract string Icon { get; }

        /// <summary>
        /// Default to an empty collection (no children)
        /// </summary>
        public virtual ILookTreeNode[] Children => new ILookTreeNode[] {};

        protected BaseTreeNode(string id)
        {
            this.Id = id;
        }    
    }
}
