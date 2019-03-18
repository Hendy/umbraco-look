using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class CulturesTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-delete-key";

        public override string Name => "Cultures";

        public override string RoutePath => "developer/lookTree/Cultures/" + this.SearcherName;

        private string SearcherName { get; }

        internal CulturesTreeNode(FormDataCollection queryStrings) : base("cultures-" + queryStrings["searcherName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
        }

        public override ILookTreeNode[] GetChildren()
        {
            var cultures = QueryService.GetCultures(this.SearcherName);

            var children = new List<CultureTreeNode>();

            foreach (var culture in cultures)
            {
                base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;
                base.QueryStrings.ReadAsNameValueCollection()["lcid"] = culture.LCID.ToString();

                children.Add(new CultureTreeNode(base.QueryStrings));
            }

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
