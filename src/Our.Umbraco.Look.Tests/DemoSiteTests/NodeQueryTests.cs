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
    public class NodeQueryTests : BaseDemoSiteTests
    {   
        [TestMethod]
        public void Not_Id()
        {
            var lookResult1 = LookService.Query(new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") });
            var lookResult2 = LookService.Query(new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") { NotIds = new int[] { 1081 } } });

            Assert.IsTrue(lookResult1.Success);
            Assert.IsTrue(lookResult2.Success);

            Assert.IsTrue(lookResult1.Total > 0);
            Assert.IsTrue(lookResult2.Total > 0);

            Assert.IsTrue(lookResult1.Total > lookResult2.Total);
        }

        [TestMethod]
        public void Not_Ids()
        {
            var lookResult1 = LookService.Query(new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") });
            var lookResult2 = LookService.Query(new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") { NotIds = new int[] { 1081, 1075 } } });

            Assert.IsTrue(lookResult1.Success);
            Assert.IsTrue(lookResult2.Success);

            Assert.IsTrue(lookResult1.Total > 0);
            Assert.IsTrue(lookResult2.Total > 0);

            Assert.IsTrue(lookResult1.Total > lookResult2.Total);
        }
    }
}
