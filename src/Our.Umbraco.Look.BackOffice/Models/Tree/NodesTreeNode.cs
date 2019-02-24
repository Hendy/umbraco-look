using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core.Models;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class NodesTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-search"; //"icon-script-alt"; // 

        public override string Name => "Nodes";

        public override string RoutePath => "developer/lookTree/Nodes/" + this.SearcherName;

        private string SearcherName { get; }

        internal NodesTreeNode(FormDataCollection queryStrings) : base("nodes-" + queryStrings["searcherName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
        }

        public override ILookTreeNode[] GetChildren()
        {
            var children = new List<NodeTypeTreeNode>();

            var queryStrings = base.QueryStrings;

            queryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;

            queryStrings.ReadAsNameValueCollection()["nodeType"] = PublishedItemType.Content.ToString();
            children.Add(new NodeTypeTreeNode(queryStrings));

            queryStrings.ReadAsNameValueCollection()["nodeType"] = PublishedItemType.Media.ToString();
            children.Add(new NodeTypeTreeNode(queryStrings));

            queryStrings.ReadAsNameValueCollection()["nodeType"] = PublishedItemType.Member.ToString();
            children.Add(new NodeTypeTreeNode(queryStrings));

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
