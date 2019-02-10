using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models.Tree
{
    internal class TagTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-tag";

        public override string Name => this.LookTag.Name + $" ({ this.Count })";

        public override string RoutePath => "developer/lookTree/Tag/" + this.SearcherName + "|" + this.LookTag.Group + "|" + this.LookTag.Name;

        private string SearcherName { get; }

        private LookTag LookTag { get; }

        /// <summary>
        /// Number of documents using this tag
        /// </summary>
        private int Count { get; }

        internal TagTreeNode(FormDataCollection queryStrings, int count = -1) : base("tag-" + queryStrings["searcherName"] + "|" + queryStrings["tagGroup"] + "|" + queryStrings["tagName"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];

            var tagGroup = queryStrings["tagGroup"];
            var tagName = queryStrings["tagName"];

            this.LookTag = new LookTag(tagGroup, tagName);

            this.Count = count;
        }
    }
}
