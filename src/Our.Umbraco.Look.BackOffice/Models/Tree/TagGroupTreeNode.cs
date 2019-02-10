using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class TagGroupTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-tags";

        public override string Name => string.IsNullOrWhiteSpace(this.TagGroup) ? "<Default>" : this.TagGroup;

        public override string RoutePath => "developer/lookTree/TagGroup/" + this.SearcherName + "|" + this.TagGroup;

        private string SearcherName { get; }

        private string TagGroup { get; }

        internal TagGroupTreeNode(FormDataCollection queryStrings) : base("tagGroup-" + queryStrings["searcherName"] + "|" + queryStrings["tagGroup"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
            this.TagGroup = queryStrings["tagGroup"];
        }

        public override ILookTreeNode[] GetChildren()
        {
            var tags = QueryService.GetTags(this.SearcherName, this.TagGroup);

            var children = new List<TagTreeNode>();

            foreach (var tag in tags)
            {
                base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;
                base.QueryStrings.ReadAsNameValueCollection()["tagGroup"] = this.TagGroup;
                base.QueryStrings.ReadAsNameValueCollection()["tagName"] = tag.Name;

                children.Add(new TagTreeNode(base.QueryStrings));
            }

            return children.ToArray();
        }
    }
}
