using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Models.Trees;

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
                base.QueryStrings.ReadAsNameValueCollection()["tagName"] = tag.Key.Name;

                children.Add(new TagTreeNode(base.QueryStrings, tag.Value)); // create tag node with count
            }

            return children.ToArray();
        }

        public override MenuItemCollection GetMenu()
        {
            var menu = new MenuItemCollection();

            menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);

            return menu;
        }
    }
}
