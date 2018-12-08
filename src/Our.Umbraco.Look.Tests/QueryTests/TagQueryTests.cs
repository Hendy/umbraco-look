using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class TagQueryTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] {
                new Thing() { Tags = TagQuery.MakeTags("shape:circle", "size:large")},
                new Thing() { Tags = TagQuery.MakeTags("shape:circle", "size:medium")},
                new Thing() { Tags = TagQuery.MakeTags("shape:circle", "size:small")},
                new Thing() { Tags = TagQuery.MakeTags("shape:square", "size:large")},
                new Thing() { Tags = TagQuery.MakeTags("shape:square", "size:medium")},
                new Thing() { Tags = TagQuery.MakeTags("shape:square", "size:small")}
            });
        }

        [TestMethod]
        public void All_Circles()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery.All = new LookTag[] { new LookTag("shape", "circle") };

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(3, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void All_Small_Or_Medium_Circles()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery.All = TagQuery.MakeTags("shape:circle");
            lookQuery.TagQuery.Any = TagQuery.MakeTags("size:small", "size:medium");

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(2, lookResult.TotalItemCount);
        }
    }
}
