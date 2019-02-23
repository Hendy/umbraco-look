using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class NodeTypeTreeNode : BaseTreeNode
    {
        public override string Icon => ""; // icon-umb-content, icon-umb-media, icon-umb-members

        public override string Name => "TODO";

        public override string RoutePath => "developer/lookTree/Nodes/" + this.SearcherName + "|" + this.NodeType;

        private string SearcherName { get; }

        private string NodeType { get; }

        internal NodeTypeTreeNode(FormDataCollection queryStrings) : base("nodeType-" + queryStrings["searcherName"] + "|" + queryStrings["nodeType"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
            this.NodeType = queryStrings["nodeType"];
        }

        public override ILookTreeNode[] GetChildren()
        {
            var children = new List<TagGroupTreeNode>();

            // if node type = content, then children of cultures

            // other child node type: detached ?

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
