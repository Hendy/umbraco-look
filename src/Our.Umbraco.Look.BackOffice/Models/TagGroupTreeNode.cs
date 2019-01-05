using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Linq;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagGroupTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-tags";

        public override string Name => this.TagGroup;

        private string SearcherName { get; }

        private string TagGroup { get; }

        internal TagGroupTreeNode(string searcherName, string tagGroup) : base("tagGroup-" + searcherName + "|" + tagGroup)
        {
            this.SearcherName = searcherName;
            this.TagGroup = tagGroup;
        }

        public override ILookTreeNode[] Children
        {
            get
            {
                return new LookQuery(this.SearcherName) { TagQuery = new TagQuery() }
                            .Run()
                            .Matches
                            .SelectMany(x => x.Tags.Where(y => y.Group == this.TagGroup))
                            .Distinct()
                            .OrderBy(x => x.Name)
                            .Select(x => new TagTreeNode(this.SearcherName, x))
                            .ToArray();
            }
        }
    }
}
