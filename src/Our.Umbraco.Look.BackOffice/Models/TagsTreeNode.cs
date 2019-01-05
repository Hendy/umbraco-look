using Examine.Providers;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagsTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-delete-key";

        public override string Name => "Tags";

        private BaseSearchProvider BaseSearchProvider { get; }

        internal TagsTreeNode(BaseSearchProvider baseSearchProvider) : base("tags-" + baseSearchProvider.Name)
        {
            this.BaseSearchProvider = baseSearchProvider;
        }
    }
}
