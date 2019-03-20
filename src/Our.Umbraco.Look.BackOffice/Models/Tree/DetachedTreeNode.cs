using System;
using System.Net.Http.Formatting;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class DetachedTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-out";

        public override string Name => "Detached";

        public override string RoutePath => "developer/lookTree/Detached/" + this.SearcherName + "|" + this.NodeType;

        private string SearcherName { get; }

        private PublishedItemType NodeType { get; }

        internal DetachedTreeNode(FormDataCollection queryStrings) : base("detached-" + queryStrings["searcherName"] + "|" + queryStrings["nodeType"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
            this.NodeType = (PublishedItemType)Enum.Parse(typeof(PublishedItemType), queryStrings["nodeType"], true);
        }
    }
}
