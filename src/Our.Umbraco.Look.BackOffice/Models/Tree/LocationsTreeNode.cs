using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class LocationsTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-map-location";

        public override string Name => "Locations";

        public override string RoutePath => "developer/lookTree/Locations/" + this.SearcherName;

        private string SearcherName { get; }

        internal LocationsTreeNode(FormDataCollection queryStrings) : base("locations-" + queryStrings["searcherName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
        }
    }
}
