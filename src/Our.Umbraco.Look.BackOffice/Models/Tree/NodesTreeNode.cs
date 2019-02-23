using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
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
            var children = new List<TagGroupTreeNode>();

            base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;

            /// <summary>
            /// The types of node to find, eg. Content, Media, Members
            /// </summary>
            //PublishedItemType.

            //base.QueryStrings.ReadAsNameValueCollection()["nodeType"] = nodeType;
            //children.Add(new NodeTypeTreeNode(base.QueryStrings));

            //children.Add(new NodeTypeTreeNode(base.QueryStrings));
            //children.Add(new NodeTypeTreeNode(base.QueryStrings));

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
