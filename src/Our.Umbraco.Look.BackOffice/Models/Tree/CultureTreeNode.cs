using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class CultureTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-chat";

        public override string Name => this.CultureInfo != null ? this.CultureInfo.Name : "Unknown";

        public override string RoutePath => "developer/lookTree/Culture/" + this.SearcherName + "|" + this.CultureInfo?.LCID;

        private string SearcherName { get; }

        private CultureInfo CultureInfo { get; }

        internal CultureTreeNode(FormDataCollection queryStrings) : base("culture-" + queryStrings["searcherName"] + "|" + queryStrings["lcid"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];

            if (int.TryParse(queryStrings["lcid"], out int lcid))
            {
                this.CultureInfo = new CultureInfo(lcid);
            }
        }
    }
}
