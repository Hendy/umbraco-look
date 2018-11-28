using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using Our.Umbraco.Look.Tests.DemoSiteTests;
using System;

namespace Our.Umbraco.Look.Tests
{
    [TestClass]
    public class QueryDemoSiteTests : BaseDemoSiteTests
    {
        [TestMethod]
        public void New_Query_Not_Compiled()
        {
            var lookQuery = new LookQuery();

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void New_Query_Executed_To_Make_Compiled()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            Assert.IsNotNull(lookQuery.Compiled);
        }


        [TestMethod]
        public void Invalidate_Compiled_By_Raw_Query_Change()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            lookQuery.RawQuery = "+field:value";

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Node_Query_Change()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            lookQuery.NodeQuery = new NodeQuery();

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Name_Query_Change()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            lookQuery.NameQuery.StartsWith = "new value";

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Date_Query_Change()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            lookQuery.DateQuery.Before = DateTime.MaxValue;

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Text_Query_Change()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            lookQuery.TextQuery.GetText = true;

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Tag_Query_Change()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            lookQuery.TagQuery.GetFacets = new string[] { };

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Location_Query_Change()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            lookQuery = LookService.Query(lookQuery).CompiledQuery;

            lookQuery.LocationQuery.MaxDistance = new Distance(1, DistanceUnit.Miles);

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Re_Execute_Compiled_Expect_Same_Results()
        {
            var lookQuery = new LookQuery(this._searchingContext) { NodeQuery = new NodeQuery("thing") };

            var results1 = LookService.Query(lookQuery);
            var results2 = LookService.Query(results1.CompiledQuery);

            Assert.IsTrue(results1.Success);
            Assert.IsTrue(results1.Total > 0);
            Assert.AreNotEqual(results1, results2);
            Assert.AreEqual(results1.Total, results2.Total);
        }
    }
}
