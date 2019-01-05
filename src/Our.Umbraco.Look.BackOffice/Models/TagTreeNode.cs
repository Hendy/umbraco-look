namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class TagTreeNode : BaseTreeNode
    {
        public override string Icon => "icon-tag";

        public override string Name => this.LookTag.Name;

        private string SearcherName { get; }

        private LookTag LookTag { get; }

        internal TagTreeNode(string searcherName, LookTag looktag) : base("tag-" + "searcherName|" + "")
        {
            this.SearcherName = searcherName;
            this.LookTag = LookTag;
        }
    }
}
