using Our.Umbraco.Look.BackOffice.Services;
using System.Linq;
using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Our.Umbraco.Look.BackOffice.Controllers
{
    [Tree("developer", "lookTree", "Look", "icon-zoom-in")]
    public class LookTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var tree = new TreeNodeCollection();

            var treeNode = LookTreeService.MakeLookTreeNode(id);

            foreach(var childTreeNode in treeNode.Children)
            {
                tree.Add(
                    this.CreateTreeNode(
                            childTreeNode.Id, 
                            id, 
                            queryStrings, 
                            childTreeNode.Name, 
                            childTreeNode.Icon, 
                            childTreeNode.Children.Any()));
            }

            return tree;

            #region OLD


            //var node = new LookTreeNode(id);

            //switch (node.Type)
            //{
            //    case LookTreeNodeType.Root:

            //        var searchProviders = ExamineManager.Instance.SearchProviderCollection;

            //        foreach (var searchProvider in searchProviders)
            //        {
            //            var baseSearchProvider = searchProvider as BaseSearchProvider;

            //            if (baseSearchProvider != null)
            //            {
            //                if (baseSearchProvider is LookSearcher)
            //                {

            //                }
            //                else // must be an Examine one (only include ones that are 'hooked' into)
            //                {


            //                    //LookService.GetExamineIndexers().Any(x => x.IndexSetName == baseSearchProvider.
            //                }
            //                // is it an Examine one or a Look one ?
            //                // always render Look ones in tree, but only Examine ones that can be 'hooked' into

            //                // if searcher has any items with tags
            //                var hasTaggedItems =
            //                    new LookQuery(baseSearchProvider.Name)
            //                    {
            //                        RawQuery = "+" + LookConstants.HasTagsField + ":1"
            //                    }
            //                    .Run()
            //                    .TotalItemCount > 0;

            //                tree.Add(this.CreateTreeNode("searchProvider-" + baseSearchProvider.Name, id, queryStrings, baseSearchProvider.Name, "icon-files", hasTaggedItems));
            //            }
            //        }

            //        break;

            //    case LookTreeNodeType.SearchProvider:

            //        var tagGroups = new LookQuery(node.SearchProvider) { RawQuery = "+" + LookConstants.HasTagsField + ":" + Boolean.TrueString.ToLower() }
            //                            .Run()
            //                            .Matches
            //                            .SelectMany(x => x.Tags.Select(y => y.Group))
            //                            .Distinct();

            //        foreach (var tagGroup in tagGroups)
            //        {
            //            tree.Add(this.CreateTreeNode(id + "-tagGroup-" + tagGroup, id, queryStrings, tagGroup != "" ? tagGroup : "Default", "icon-tags", true));
            //        }

            //        break;

            //    case LookTreeNodeType.TagGroup:

            //        var tags = new LookQuery(node.SearchProvider) { RawQuery = "+" + LookConstants.HasTagsField + ":" + Boolean.TrueString.ToLower() }
            //                            .Run()
            //                            .Matches
            //                            .SelectMany(x => x.Tags.Where(y => y.Group == node.TagGroup))
            //                            .Select(x => x.Name)
            //                            .Distinct();

            //        foreach (var tag in tags)
            //        {
            //            tree.Add(this.CreateTreeNode(id + "-tag-" + tag, id, queryStrings, tag, "icon-tag", false));
            //        }

            //        break;
            //}
            #endregion
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            MenuItemCollection nodeMenu = new MenuItemCollection();

            return nodeMenu;
        }
    }
}
