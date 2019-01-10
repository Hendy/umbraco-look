using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class LookQueryConflictTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // create an index (doesn't need any data) but it needs to exist so we can get context
            TestHelper.IndexThings(new Thing[] { });
        }

        [TestMethod]
        public void Key_Conflict()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NodeQuery = new NodeQuery()
            {
                Keys = new string[] { "04719e28-a337-4710-be12-9764afba8096" },
                NotKeys = new string[] { "04719e28-a337-4710-be12-9764afba8096" }
            };

            var lookResult = lookQuery.Run();

            Assert.IsNotNull(lookResult);
            Assert.IsFalse(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 0);
        }

        [TestMethod]
        public void Tag_All_Conflict()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                All = TagQuery.MakeTags("tag1"),
                None = TagQuery.MakeTags("tag1")
            };

            var lookResult = lookQuery.Run();

            Assert.IsNotNull(lookResult);
            Assert.IsFalse(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 0);
        }

        [TestMethod]
        public void Tag_Any_Conflict()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                Any = TagQuery.MakeTags("tag1"),
                None = TagQuery.MakeTags("tag1")
            };

            var lookResult = lookQuery.Run();

            Assert.IsNotNull(lookResult);
            Assert.IsFalse(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 0);
        }
    }
}
