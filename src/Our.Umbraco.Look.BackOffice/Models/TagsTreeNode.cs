using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagsTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-delete-key";

        public override string Name => "Tags";

        private string SearcherName { get; }

        internal TagsTreeNode(FormDataCollection queryStrings) : base("tags-" + queryStrings["searcherName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
        }

        public override ILookTreeNode[] GetChildren()
        {
            var tagGroups = new LookQuery(this.SearcherName) { TagQuery = new TagQuery() }
                                .Run()
                                .Matches
                                .SelectMany(x => x.Tags.Select(y => y.Group))
                                .Distinct()
                                .OrderBy(x => x);

            var children = new List<TagGroupTreeNode>();

            foreach (var tagGroup in tagGroups)
            {
                base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;
                base.QueryStrings.ReadAsNameValueCollection()["tagGroup"] = tagGroup;

                children.Add(new TagGroupTreeNode(base.QueryStrings));
            }

            return children.ToArray();
        }
    }
}
