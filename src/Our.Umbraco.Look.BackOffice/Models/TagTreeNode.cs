using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-tag";

        public override string Name => this.LookTag.Name;

        private string SearcherName { get; }

        private LookTag LookTag { get; }

        internal TagTreeNode(FormDataCollection queryStrings) : base("tag-" + queryStrings["searcherName"] + "|" + queryStrings["tag"], queryStrings)
        {
            this.SearcherName = queryStrings["searcherName"];
            this.LookTag = new LookTag(queryStrings["tag"]);
        }
    }
}
