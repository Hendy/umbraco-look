using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System.Configuration;
using System.IO;

namespace Our.Umbraco.Look.Tests.DemoSiteTests
{
    [TestClass]
    public abstract class BaseDemoSiteTests
    {
        internal SearchingContext _searchingContext;

        [TestInitialize]
        public void BaseInitialize()
        {
            this._searchingContext = new SearchingContext()
            {
                Analyzer = new WhitespaceAnalyzer(),
                EnableLeadingWildcards = true,
                IndexSearcher = new IndexSearcher(new SimpleFSDirectory(new DirectoryInfo(ConfigurationManager.AppSettings["DemoSiteLuceneDirectory"])), true)
            };
        }
    }
}
