using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class NameQueryTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] {
                new Thing() { Name = "123" },
                new Thing() { Name = "xyz" },
                new Thing() { Name = "ABC" }
            });
        }

        [TestMethod]
        public void Has_Name()
        {
            Assert.IsTrue(new LookQuery(TestHelper.GetSearchingContext()) { NameQuery = new NameQuery() }.Run().TotalItemCount > 0);
        }

        [TestMethod]
        public void Is()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery();
            lookQuery.NameQuery.Is = "123";

            var lookResult = lookQuery.Run();

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 1);
            Assert.AreEqual("123", (lookResult.Matches.First()).Name);
        }

        [TestMethod]
        public void Is_And_Starts_With()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery();
            lookQuery.NameQuery.Is = "123";
            lookQuery.NameQuery.StartsWith = "12";

            var lookResult = lookQuery.Run();

            Assert.IsTrue(lookResult.TotalItemCount > 0);
        }

        [TestMethod]
        public void Is_And_Ends_With()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery();
            lookQuery.NameQuery.Is = "123";
            lookQuery.NameQuery.EndsWith= "23";

            var lookResult = lookQuery.Run();

            Assert.IsTrue(lookResult.TotalItemCount > 0);
        }

        [TestMethod]
        public void Is_And_Starts_With_And_Ends_With()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery();
            lookQuery.NameQuery.Is = "123";
            lookQuery.NameQuery.StartsWith = "12";
            lookQuery.NameQuery.EndsWith = "23";

            var lookResult = lookQuery.Run();

            Assert.IsTrue(lookResult.TotalItemCount > 0);
        }

        [TestMethod]
        public void Conflicting_Query_Is_And_Starts_With()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery();
            lookQuery.NameQuery.Is = "123";
            lookQuery.NameQuery.StartsWith = "xyz";

            var lookResult = lookQuery.Run();

            Assert.IsFalse(lookResult.Success);
        }

        [TestMethod]
        public void Conflicting_Query_Is_And_Ends_With()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery = new NameQuery();
            lookQuery.NameQuery.Is = "123";
            lookQuery.NameQuery.EndsWith = "xyz";

            var lookResult = lookQuery.Run();

            Assert.IsFalse(lookResult.Success);
        }

    }
}
