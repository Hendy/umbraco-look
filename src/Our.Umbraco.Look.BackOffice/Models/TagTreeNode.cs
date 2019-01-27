using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-tag";

        public override string Name => this.LookTag.Name;

        public override string RoutePath => "developer/lookTree/Tag/" + this.SearcherName + "|" + this.LookTag.Group + "|" + this.LookTag.Name;

        private string SearcherName { get; }

        private LookTag LookTag { get; }

        internal TagTreeNode(FormDataCollection queryStrings) : base("tag-" + queryStrings["searcherName"] + "|" + queryStrings["tagGroup"] + "|" + queryStrings["tagName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];

            var tagGroup = queryStrings["tagGroup"];
            var tagName = queryStrings["tagName"];

            this.LookTag = new LookTag(tagGroup, tagName);
        }
    }
}
