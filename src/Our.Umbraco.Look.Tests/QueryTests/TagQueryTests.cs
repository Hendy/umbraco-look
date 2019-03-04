using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class TagQueryTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] {
                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:circle", "size:large")},
                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:circle", "size:medium")},
                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:circle", "size:small")},

                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:square", "size:large")},
                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:square", "size:medium")},
                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:square", "size:small")},

                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:oblong", "size:large")},
                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:oblong", "size:medium")},
                new Thing() { Tags = TagQuery.MakeTags("shape", "shape:oblong", "size:small")}
            });
        }

        [TestMethod]
        public void Has_Tags()
        {
            Assert.IsTrue(new LookQuery(TestHelper.GetSearchingContext()) { TagQuery = new TagQuery() }.Search().TotalItemCount > 0);
        }

        [TestMethod]
        public void All_Circles()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery() { HasAll = new [] { new LookTag("shape", "circle") } };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(3, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void All_Small_Or_Medium_Circles()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                HasAll = TagQuery.MakeTags("shape:circle"),
                HasAnyOr = new LookTag[][] { TagQuery.MakeTags("size:small", "size:medium") }
            };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(2, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void All_Small_Or_Medium_Circles_Or_Oblongs()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                HasAnyOr = new LookTag[][] {
                        TagQuery.MakeTags("shape:circle", "shape:oblong"), // either of these
                        TagQuery.MakeTags("size:small", "size:medium") // and either of these
                }
            };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(4, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void All_Shapes_Except_Circles()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                HasAll = TagQuery.MakeTags("shape"),
                NotAny = TagQuery.MakeTags("shape:circle")
            };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(6, lookResult.TotalItemCount);
        }
    }
}
