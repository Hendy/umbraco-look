using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core.Models;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class NodeTypeTreeNode : BaseTreeNode
    {
        public override string Icon =>
            this.NodeType == PublishedItemType.Content ? "icon-umb-content"
            : this.NodeType == PublishedItemType.Media ? "icon-umb-media"
            : this.NodeType == PublishedItemType.Member ? "icon-umb-members"
            : "";

        public override string Name =>
            this.NodeType == PublishedItemType.Content ? "Content"
            : this.NodeType == PublishedItemType.Media ? "Media"
            : this.NodeType == PublishedItemType.Member ? "Members"
            : "";

        public override string RoutePath => "developer/lookTree/NodeType/" + this.SearcherName + "|" + this.NodeType;

        private string SearcherName { get; }

        private PublishedItemType NodeType { get; }

        internal NodeTypeTreeNode(FormDataCollection queryStrings) : base("nodeType-" + queryStrings["searcherName"] + "|" + queryStrings["nodeType"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
            this.NodeType = (PublishedItemType)Enum.Parse(typeof(PublishedItemType), queryStrings["nodeType"], true);
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

            return menu;
        }
    }
}
