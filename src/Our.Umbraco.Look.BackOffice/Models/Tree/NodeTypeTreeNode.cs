using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Umbraco.Core.Models;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class NodeTypeTreeNode : BaseTreeNode
    {
        public override string Icon { get; }

        public override string Name { get; }
            
        public override string RoutePath => "developer/lookTree/NodeType/" + this.SearcherName + "|" + this.NodeType;

        private string SearcherName { get; }

        private PublishedItemType NodeType { get; }

        internal NodeTypeTreeNode(FormDataCollection queryStrings) : base("nodeType-" + queryStrings["searcherName"] + "|" + queryStrings["nodeType"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
            this.NodeType = (PublishedItemType)Enum.Parse(typeof(PublishedItemType), queryStrings["nodeType"], true);

            this.Icon = IconService.GetNodeTypeIcon(this.NodeType);

            this.Name = this.NodeType == PublishedItemType.Content ? "Content"
                        : this.NodeType == PublishedItemType.Media ? "Media"
                        : this.NodeType == PublishedItemType.Member ? "Members"
                        : null;
        }

        public override ILookTreeNode[] GetChildren()
        {
            var children = new List<ILookTreeNode>();

            // content nodes can be associated with a culture - only show if there's more than 1
            if (this.NodeType == PublishedItemType.Content && QueryService.GetCultures(this.SearcherName).Length > 1)
            {
                children.Add(new CulturesTreeNode(base.QueryStrings));
            }

            return children.ToArray();
        }

        public override MenuItemCollection GetMenu()
        {
            var menu = new MenuItemCollection();

            return menu;
        }
    }
}
