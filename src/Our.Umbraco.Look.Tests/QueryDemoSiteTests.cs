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

        [TestMethod]
        public void Get_Compiled_Query()
        {
            var lookQuery = new LookQuery();

            Assert.IsNull(lookQuery.Compiled);

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            lookQuery = lookResult.LookQuery; // the returned query has been compiled

            Assert.IsNotNull(lookResult.LookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_Query_With_Node_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            lookQuery = lookResult.LookQuery; // the returned query has been compiled

            lookQuery.NodeQuery.TypeAliases = new string[] { "diffentThing" } ;

            Assert.IsNull(lookResult.LookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_Query_With_Name_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            lookQuery = lookResult.LookQuery; // the returned query has been compiled

            lookQuery.NameQuery.StartsWith = "thing"; // chaning any property should invalidate it

            Assert.IsNull(lookResult.LookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_Query_With_Date_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            lookQuery = lookResult.LookQuery; // the returned query has been compiled

            lookQuery.DateQuery.After = System.DateTime.MaxValue;

            Assert.IsNull(lookResult.LookQuery.Compiled);
        }

    }
}
