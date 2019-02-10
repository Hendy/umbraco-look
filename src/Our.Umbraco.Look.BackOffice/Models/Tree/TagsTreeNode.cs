using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System.Collections.Generic;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class TagsTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-delete-key";

        public override string Name => "Tags";

        public override string RoutePath => "developer/lookTree/Tags/" + this.SearcherName;

        private string SearcherName { get; }

        internal TagsTreeNode(FormDataCollection queryStrings) : base("tags-" + queryStrings["searcherName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
        }

        public override ILookTreeNode[] GetChildren()
        {
            var tagGroups = QueryService.GetTagGroups(this.SearcherName);

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
