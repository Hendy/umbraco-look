using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class TagFacetQueryComparrisonTests
    {
        [TestMethod]
        public void Same_Groups_Same_Order()
        {
            var tagFacetQuery1 = new TagFacetQuery("tagGroup1", "tagGroup2");
            var tagFacetQuery2 = new TagFacetQuery("tagGroup1", "tagGroup2");

            Assert.AreEqual(tagFacetQuery1, tagFacetQuery2);
        }

        [TestMethod]
        public void Same_Groups_Different_Order()
        {
            var tagFacetQuery1 = new TagFacetQuery("tagGroup1", "tagGroup2");
            var tagFacetQuery2 = new TagFacetQuery("tagGroup2", "tagGroup1");

            Assert.AreEqual(tagFacetQuery1, tagFacetQuery2);
        }

        [TestMethod]
        public void Different_Groups()
        {
            var tagFacetQuery1 = new TagFacetQuery("tagGroup1");
            var tagFacetQuery2 = new TagFacetQuery("tagGroup2");

            Assert.AreNotEqual(tagFacetQuery1, tagFacetQuery2);
        }

        [TestMethod]
        public void Null_Null()
        {
            var tagFacetQuery1 = new TagFacetQuery() { TagGroups = null };
            var tagFacetQuery2 = new TagFacetQuery() { TagGroups = null };

            Assert.AreEqual(tagFacetQuery1, tagFacetQuery2);
        }

        [TestMethod]
        public void Null_Groups()
        {
            var tagFacetQuery1 = new TagFacetQuery() { TagGroups = null };
            var tagFacetQuery2 = new TagFacetQuery("tagGroup1");

            Assert.AreNotEqual(tagFacetQuery1, tagFacetQuery2);
        }

        [TestMethod]
        public void Groups_Null()
        {
            var tagFacetQuery1 = new TagFacetQuery("tagGroup1");
            var tagFacetQuery2 = new TagFacetQuery() { TagGroups = null };

            Assert.AreNotEqual(tagFacetQuery1, tagFacetQuery2);
        }
    }
}
