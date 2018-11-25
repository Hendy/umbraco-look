using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;

namespace Our.Umbraco.Look.Tests.DemoSiteTests
{
    [TestClass]
    public class QueryDemoSiteTests : BaseDemoSiteTests
    {
        /// <summary>
        /// Query to return any content of docType 'thing'
        /// </summary>
        [TestMethod]
        public void Get_A_Thing()
        {
            var lookQuery = new LookQuery();

            //lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };
            lookQuery.NodeQuery = new NodeQuery("thing");
            
            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
        }
    }
}
