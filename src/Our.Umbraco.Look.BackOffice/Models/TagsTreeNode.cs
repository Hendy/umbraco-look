using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Linq;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagsTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-delete-key";

        public override string Name => "Tags";

        private string SearcherName { get; }

        internal TagsTreeNode(string searcherName, FormDataCollection queryStrings) : base("tags-" + searcherName, queryStrings)
        {
            this.SearcherName = searcherName;
        }

        public override ILookTreeNode[] GetChildren()
        {
            return new LookQuery(this.SearcherName) { TagQuery = new TagQuery() }
                        .Run()
                        .Matches
                        .SelectMany(x => x.Tags.Select(y => y.Group))
                        .Distinct()
                        .Select(x => new TagGroupTreeNode(this.SearcherName, x, base.QueryStrings))
                        .ToArray();
        }
    }
}
