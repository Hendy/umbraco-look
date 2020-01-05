using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class LookQueryTests
    {
        /// <summary>
        /// Ensure there are 100 things in the index
        /// </summary>
        /// <param name="testContext"></param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(
                Enumerable
                .Range(1, 100)
                .Select(x => new Thing()));
        }

        [TestMethod]
        public void Empty_Query()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            Assert.IsFalse(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 0);
        }

        [TestMethod]
        public void Query_With_Node_Type_Clause()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NodeQuery = new NodeQuery();
            lookQuery.NodeQuery.TypeAny = new PublishedItemType[] { PublishedItemType.Content };

            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
        }

        [TestMethod]
        public void Max_Results_Value_Valid()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery(); // set a query clause so it's acutally executed
            
            lookQuery.MaxResults = 5;

            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            //Assert.AreEqual(5, lookResult.TotalItemCount);
            Assert.AreEqual(5, lookResult.Matches.Count());
        }

        [TestMethod]
        public void Max_Results_Value_Invalid_Negative()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery(); // set a query clause so it's acutally executed

            lookQuery.MaxResults = -1;

            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            //Assert.AreEqual(100, lookResult.TotalItemCount);
            Assert.AreEqual(100, lookResult.Matches.Count());
        }

    }
}
