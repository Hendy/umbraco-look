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
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };
            //lookQuery.NodeQuery = new NodeQuery("thing");

            var lookResult = LookService.Query(lookQuery, this._searchingContext);

            Assert.IsNotNull(lookResult.CompiledQuery.Compiled);
        }


        [TestMethod]
        public void Invalidate_Compiled_By_Raw_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).CompiledQuery;

            lookQuery.RawQuery = "+field:value";

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Node_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).CompiledQuery;

            lookQuery.NodeQuery = new NodeQuery(); // reset the original

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Name_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).CompiledQuery;

            lookQuery.NameQuery.StartsWith = "new value";

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Date_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).CompiledQuery;

            lookQuery.DateQuery.Before = DateTime.MaxValue;

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Text_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).CompiledQuery;

            lookQuery.TextQuery.GetText = true;

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Tag_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).CompiledQuery;

            lookQuery.TagQuery.GetFacets = new string[] { };

            Assert.IsNull(lookQuery.Compiled);
        }

        [TestMethod]
        public void Invalidate_Compiled_By_Location_Query_Change()
        {
            var lookQuery = new LookQuery();

            lookQuery.NodeQuery.TypeAliases = new string[] { "thing" };

            lookQuery = LookService.Query(lookQuery, this._searchingContext).CompiledQuery;

            lookQuery.LocationQuery.MaxDistance = new Distance(1, DistanceUnit.Miles);

            Assert.IsNull(lookQuery.Compiled);
        }

    }
}
