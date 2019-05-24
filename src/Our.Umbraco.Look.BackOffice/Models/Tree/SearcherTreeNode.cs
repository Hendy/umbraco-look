using Examine;
using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class SearcherTreeNode : BaseTreeNode
    {
        public override string Name => this.SearcherName;

        public override string Icon { get; } // could be "icon-file-cabinet, icon-files, icon-categories

        public override string RoutePath => "developer/lookTree/Searcher/" + this.SearcherName;

        private string SearcherName { get; }

        /// <summary>
        /// Flag to indicate whether Look is 'hooked' in with this Examine provider or if this is a Look provider
        /// </summary>
        private bool Active { get; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        internal SearcherTreeNode(FormDataCollection queryStrings) : base("searcher-" + queryStrings["searcherName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];

            var searcher = ExamineManager.Instance.SearchProviderCollection[this.SearcherName];

            if (searcher is LookSearcher)
            {
                this.Active = true;
            }
            else // must be an examine one
            {
                var name = this.SearcherName.TrimEnd("Searcher");

                if (LookConfiguration.ExamineIndexers.Select(x => x.TrimEnd("Indexer")).Any(x => x == name))
                {
                    this.Active = true;
                }
            }

            this.Icon = IconService.GetSearcherIcon(searcher);
        }

        public override ILookTreeNode[] GetChildren()
        {
            if (this.Active)
            {
                //base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;

                var childTreeNodes = new List<ILookTreeNode>();

                if (new LookQuery(this.SearcherName) { NodeQuery = new NodeQuery() { TypeAny = new[] { ItemType.Content, ItemType.DetachedContent } } }.Search().TotalItemCount > 0)
                {
                    base.QueryStrings.ReadAsNameValueCollection()["nodeType"] = PublishedItemType.Content.ToString();
                    childTreeNodes.Add(new NodeTypeTreeNode(base.QueryStrings));
                }

                if (new LookQuery(this.SearcherName) { NodeQuery = new NodeQuery() { TypeAny = new[] { ItemType.Media, ItemType.DetachedMedia } } }.Search().TotalItemCount > 0)
                {
                    base.QueryStrings.ReadAsNameValueCollection()["nodeType"] = PublishedItemType.Media.ToString();
                    childTreeNodes.Add(new NodeTypeTreeNode(base.QueryStrings));
                }

                if (new LookQuery(this.SearcherName) { NodeQuery = new NodeQuery() { TypeAny = new[] { ItemType.Member, ItemType.DetachedMember } } }.Search().TotalItemCount > 0)
                {
                    base.QueryStrings.ReadAsNameValueCollection()["nodeType"] = PublishedItemType.Member.ToString();
                    childTreeNodes.Add(new NodeTypeTreeNode(base.QueryStrings));
                }

                if (new LookQuery(this.SearcherName) { TagQuery = new TagQuery() }.Search().TotalItemCount > 0)
                {
                    childTreeNodes.Add(new TagsTreeNode(base.QueryStrings));
                }

                if (new LookQuery(this.SearcherName) { LocationQuery = new LocationQuery() }.Search().TotalItemCount > 0)
                {
                    childTreeNodes.Add(new LocationsTreeNode(base.QueryStrings));
                }

                return childTreeNodes.ToArray();
            }

            return base.GetChildren(); // empty
        }

        public override MenuItemCollection GetMenu()
        {
            var menu = new MenuItemCollection();

            menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);

            if (this.Active)
            {
                menu.Items.Add(new MenuItem("Rebuild", "Rebuild index") { Icon = "axis-rotation", SeperatorBefore = true });
            }

            return menu;
        }

    }
}
