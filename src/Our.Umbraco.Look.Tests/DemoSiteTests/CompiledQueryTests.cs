using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using Our.Umbraco.Look.Tests.DemoSiteTests;

namespace Our.Umbraco.Look.Tests
{
    [TestClass]
    public class QueryDemoSiteTests : BaseDemoSiteTests
    {

        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestMethod]
        public void New_Query_Not_Compiled()
        {
            var lookQuery = new LookQuery();

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void New_Query_Executed_To_Make_Compiled()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };
            //lookQuery.NodeQuery = new NodeQuery("thing");

            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            Assert.IsNotNull(lookResult.LookQuery.Compiled);
        }


        [TestMethod]
        public void Invalidate_Compiled_By_Raw_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).LookQuery;

            lookQuery.RawQuery = "+field:value";

            Assert.IsNull(lookQuery.Compiled);
        }


        public void Invalidate_Compiled_By_Node_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).LookQuery;

            lookQuery.NodeQuery = new NodeQuery();

            Assert.IsNull(lookQuery.Compiled);
        }
    }
}
