using Examine.Providers;
using Our.Umbraco.Look.BackOffice.Interfaces;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class SearcherTreeNode : BaseTreeNode
    {
        public override string Name { get; }

        public override string Icon { get; } // could be "icon-file-cabinet, icon-files, icon-categories

        private BaseSearchProvider BaseSearchProvider { get; }

        /// <summary>
        /// Flag to indicate whether Look is 'hooked' in with this Examine provider or if this is a Look provider
        /// </summary>
        private bool Active { get; } = false;

        internal SearcherTreeNode(BaseSearchProvider baseSearchProvider) : base("searcher-" + baseSearchProvider.Name)
        {
            this.BaseSearchProvider = baseSearchProvider;

            this.Name = baseSearchProvider.Name;

            if (baseSearchProvider is LookSearcher)
            {
                this.Icon = "icon-categories";
                this.Active = true;
            }
            else // must be an examine one
            {
                // TODO: is Look 'hooked' into this examine searcher
                
                this.Icon = "icon-file-cabinet";
            }
        }

        public override ILookTreeNode[] Children
        {
            get
            {
                if (this.Active)
                {
                    return new ILookTreeNode[] { new TagsTreeNode(this.BaseSearchProvider) };
                }

                return base.Children; // empty
            }
        }
    }
}
