using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Umbraco.Core.Models;

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

            if (new LookQuery(this.SearcherName) { NodeQuery = new NodeQuery() { Type = this.NodeType, DetachedQuery = DetachedQuery.OnlyDetached } }.Search().TotalItemCount > 0)
            {
                base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;
                base.QueryStrings.ReadAsNameValueCollection()["nodeType"] = this.NodeType.ToString();

                children.Add(new DetachedTreeNode(base.QueryStrings));
            }

            return children.ToArray();
        }
    }
}
