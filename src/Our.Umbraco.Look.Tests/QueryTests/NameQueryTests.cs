using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class NameQueryTests : BaseQueryTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //TestHelper.DeleteIndex();

            TestHelper.IndexThings(new Thing[] {
                new Thing() { Name = "123" },
                new Thing() { Name = "xyz" },
                new Thing() { Name = "ABC" }
            });
        }


        [TestMethod]
        public void Is_And_Starts_With()
        {
            var lookQuery = new LookQuery(this._searchingContext);

            lookQuery.NameQuery.Is = "123";
            lookQuery.NameQuery.StartsWith = "12";

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Total > 0);
        }


    }
}
