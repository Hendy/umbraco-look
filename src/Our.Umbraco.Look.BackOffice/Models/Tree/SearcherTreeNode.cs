using Examine;
using Examine.Providers;
using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Linq;
using System.Net.Http.Formatting;
using Umbraco.Core;

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
                this.Icon = "icon-files";
            }
            else // must be an examine one
            {
                var name = this.SearcherName.TrimEnd("Searcher");

                if (LookConfiguration.ExamineIndexers.Select(x => x.TrimEnd("Indexer")).Any(x => x == name))
                {
                    this.Active = true;
                    this.Icon = "icon-categories";
                }
                else
                {
                    this.Icon = "icon-file-cabinet";
                }
            }
        }

        public override ILookTreeNode[] GetChildren()
        {
            if (this.Active)
            {
                //base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = this.SearcherName;

                return new ILookTreeNode[] { new TagsTreeNode(base.QueryStrings) };
            }

            return base.GetChildren(); // empty
        }
    }
}
