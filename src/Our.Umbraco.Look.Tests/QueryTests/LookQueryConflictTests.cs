using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class LookQueryConflictTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // create an index (doesn't need any data) but it needs to exist so we can get context
            TestHelper.IndexThings(new Thing[] {
                new Thing() { Tags = TagQuery.MakeTags("tag1") },
                new Thing() { Tags = TagQuery.MakeTags("tag2") }
            });
        }

        [TestMethod]
        public void Key_Conflict()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NodeQuery = new NodeQuery()
            {
                Keys = new [] { Guid.Parse("04719e28-a337-4710-be12-9764afba8096") },
                NotKeys = new [] { Guid.Parse("04719e28-a337-4710-be12-9764afba8096") },
            };

            var lookResult = lookQuery.Search();

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
                HasAll = TagQuery.MakeTags("tag1"),
                NotAny = TagQuery.MakeTags("tag1")
            };

            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            //Assert.IsFalse(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 0);
        }

        [TestMethod]
        public void Tag_Any_Conflict()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                HasAnyAnd = new LookTag[][] { TagQuery.MakeTags("tag1") },
                NotAny = TagQuery.MakeTags("tag1")
            };

            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            //Assert.IsFalse(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 0);
        }
    }
}
