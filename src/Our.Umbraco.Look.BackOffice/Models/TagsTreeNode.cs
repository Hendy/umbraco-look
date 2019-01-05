using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Linq;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagsTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-delete-key";

        public override string Name => "Tags";

        private string SearcherName { get; }

        internal TagsTreeNode(string searcherName) : base("tags-" + searcherName)
        {
            this.SearcherName = searcherName;
        }

        public override ILookTreeNode[] Children
        {
            get
            {
                return new LookQuery(this.SearcherName) { TagQuery = new TagQuery() }
                            .Run()
                            .Matches
                            .SelectMany(x => x.Tags.Select(y => y.Group))
                            .Distinct()
                            .Select(x => new TagGroupTreeNode(this.SearcherName, x))
                            .ToArray();
            }
        }
    }
}
