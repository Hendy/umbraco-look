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
        public void Has()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery() { Has = new LookTag("shape") };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(9, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void Not()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery() {
                Has = new LookTag("shape"), // HACK: need to clear index on startup of this class
                Not = new LookTag("shape", "circle")
            };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(6, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void Has_All()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery() { HasAll = new [] { new LookTag("shape", "circle") } };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(3, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void Has_Any()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                HasAny = TagQuery.MakeTags("shape:circle", "shape:square")
            };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(6, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void Has_Any_And()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery()
            {
                HasAnyAnd = new LookTag[][] {
                        TagQuery.MakeTags("shape:circle", "shape:oblong"), // either of these
                        TagQuery.MakeTags("size:small", "size:medium") // and either of these
                }
            };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(4, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void Not_Any()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery() {
                Has = new LookTag("shape"), // HACK: need to clear index on startup of this class
                NotAny = TagQuery.MakeTags("shape:circle", "shape:oblong")
            };

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(3, lookResult.TotalItemCount);
        }

        // all shapes except circles
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
