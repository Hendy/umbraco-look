using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class TagQueryComparrisonTests
    {
        [TestMethod]
        public void Same_Tags_Same_Order()
        {
            var tagQuery1 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag2") };
            var tagQuery2 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag2") };

            Assert.AreEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Same_Tags_Different_Order()
        {
            var tagQuery1 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag2") };
            var tagQuery2 = new TagQuery() { All = TagQuery.MakeTags("tag2", "tag1") };

            Assert.AreEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Different_Tags()
        {
            var tagQuery1 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag2") };
            var tagQuery2 = new TagQuery() { All = TagQuery.MakeTags("tag2", "tag3") };

            Assert.AreNotEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Null_Null()
        {
            var tagQuery1 = new TagQuery() { All = null };
            var tagQuery2 = new TagQuery() { All = null };

            Assert.AreEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Tags_Null()
        {
            var tagQuery1 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag2") };
            var tagQuery2 = new TagQuery() { All = null };

            Assert.AreNotEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Null_Tags()
        {
            var tagQuery1 = new TagQuery() { All = null };
            var tagQuery2 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag2") };

            Assert.AreNotEqual(tagQuery1, tagQuery2);

        }

        [TestMethod]
        public void Same_Tags_Different_Quantity()
        {
            var tagQuery1 = new TagQuery() { All = TagQuery.MakeTags("tag1") };
            var tagQuery2 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag1") };

            Assert.AreNotEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Same_Tags_Different_Quantity_Alternate()
        {
            var tagQuery1 = new TagQuery() { All = TagQuery.MakeTags("tag1", "tag1") };
            var tagQuery2 = new TagQuery() { All = TagQuery.MakeTags("tag1") };

            Assert.AreNotEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Same_Tag_Collection_Of_Collections()
        {
            var tagQuery1 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag1", "tag2"), TagQuery.MakeTags("tag3", "tag4") }};
            var tagQuery2 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag1", "tag2"), TagQuery.MakeTags("tag3", "tag4") }};

            Assert.AreEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Same_Tag_Collection_Of_Collections_Out_Of_Order()
        {
            var tagQuery1 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag1", "tag2"), TagQuery.MakeTags("tag3", "tag4") }};
            var tagQuery2 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag3", "tag4"), TagQuery.MakeTags("tag1", "tag2") }};

            Assert.AreEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Same_Tag_Collection_Of_Collections_Mixed_Up()
        {
            var tagQuery1 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag2", "tag1"), TagQuery.MakeTags("tag3", "tag4") } };
            var tagQuery2 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag3", "tag4"), TagQuery.MakeTags("tag1", "tag2") } };

            Assert.AreEqual(tagQuery1, tagQuery2);
        }

        [TestMethod]
        public void Different_Tag_Collection_Of_Collections()
        {
            var tagQuery1 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag1", "tag2"), TagQuery.MakeTags("tag3", "tag4") } };
            var tagQuery2 = new TagQuery() { Any = new LookTag[][] { TagQuery.MakeTags("tag1", "tag2") } };

            Assert.AreNotEqual(tagQuery1, tagQuery2);
        }
    }
}
