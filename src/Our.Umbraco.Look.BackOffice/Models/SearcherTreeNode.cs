using Examine;
using Examine.Providers;
using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Linq;
using System.Net.Http.Formatting;
using Umbraco.Core;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class SearcherTreeNode : BaseTreeNode
    {
        public override string Name => this.SearcherName;

        public override string Icon { get; } // could be "icon-file-cabinet, icon-files, icon-categories

        private BaseSearchProvider BaseSearchProvider { get; }

        private string SearcherName { get; }

        /// <summary>
        /// Flag to indicate whether Look is 'hooked' in with this Examine provider or if this is a Look provider
        /// </summary>
        private bool Active { get; } = false;

        /// <summary>
        /// Constrcut
        /// </summary>
        /// <param name="baseSearchProvider"></param>
        internal SearcherTreeNode(string searcherName, FormDataCollection queryStrings) : base("searcher-" + searcherName, queryStrings)
        {
            this.SearcherName = searcherName;

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];

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
                return new ILookTreeNode[] { new TagsTreeNode(this.SearcherName, base.QueryStrings) };
            }

            return base.GetChildren(); // empty
        }
    }
}
