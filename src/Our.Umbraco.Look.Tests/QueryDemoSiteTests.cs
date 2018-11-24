using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System.Configuration;
using System.IO;

namespace Our.Umbraco.Look.Tests
{
    [TestClass]
    public class QueryDemoSiteTests
    {
        SearchingContext _searchingContext = new SearchingContext()
        {
            Analyzer = new WhitespaceAnalyzer(),
            EnableLeadingWildcards = true,
            IndexSearcher = new IndexSearcher(new SimpleFSDirectory(new DirectoryInfo(ConfigurationManager.AppSettings["DemoSiteLuceneDirectory"])),true)
        };

        /// <summary>
        /// Query to return any content of docType 'thing'
        /// </summary>
        [TestMethod]
        public void Get_A_Thing()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };
            
            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
        }


    }
}
