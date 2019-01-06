using Examine;
using Examine.Providers;
using Our.Umbraco.Look.BackOffice.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Models
{
    internal class RootTreeNode : BaseTreeNode
    {
        public override string Name => string.Empty; //null; //"Look";

        public override string Icon => string.Empty; //null; //"icon-zoom-in";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        internal RootTreeNode(string id, FormDataCollection queryStrings) : base(id, queryStrings) { }

        /// <summary>
        /// For each examine searcher (Examine & Look) create a child node
        /// </summary>
        public override ILookTreeNode[] GetChildren()
        {
            var children = new List<ILookTreeNode>();

            var searchProviders = ExamineManager.Instance.SearchProviderCollection;

            foreach (var searchProvider in searchProviders)
            {
                var baseSearchProvider = searchProvider as BaseSearchProvider;

                if (baseSearchProvider != null) // safety check
                {
                    base.QueryStrings.ReadAsNameValueCollection()["searcherName"] = baseSearchProvider.Name;

                    children.Add(new SearcherTreeNode(base.QueryStrings));
                }
            }

            return children.OrderBy(x => x.Name).ToArray();
        }
    }
}
