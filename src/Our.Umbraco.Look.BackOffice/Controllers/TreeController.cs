using Our.Umbraco.Look.BackOffice.Services;
using System.Linq;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using UmbracoTreeController = Umbraco.Web.Trees.TreeController;

namespace Our.Umbraco.Look.BackOffice.Controllers
{
    [Tree("developer", "lookTree", "Look", "icon-zoom-in")]
    [PluginController("Look")]
    public class TreeController : UmbracoTreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var tree = new TreeNodeCollection();

            var treeNode = LookTreeService.MakeLookTreeNode(id, queryStrings);

            foreach(var childTreeNode in treeNode.GetChildren())
            {
                tree.Add(
                    this.CreateTreeNode(
                            childTreeNode.Id, 
                            id, 
                            childTreeNode.QueryStrings, 
                            childTreeNode.Name, 
                            childTreeNode.Icon, 
                            childTreeNode.GetChildren().Any(),
                            childTreeNode.RoutePath));
            }

            return tree;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            MenuItemCollection nodeMenu = new MenuItemCollection();

            if (!id.StartsWith("tag-"))
            {
                nodeMenu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            }

            return nodeMenu;
        }
    }
}
