using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class TagFacetQueryTests
    {
        [TestMethod]
        public void Empty_Constructor_Default()
        {
            var tagFacetQuery = new TagFacetQuery();

            Assert.IsNotNull(tagFacetQuery.TagGroups);
            Assert.IsNotNull(tagFacetQuery.TagGroups.Length == 1);
            Assert.AreEqual("", tagFacetQuery.TagGroups.Single());
        }

        [TestMethod]
        public void Empty_String_Contructor()
        {
            var tagFacetQuery = new TagFacetQuery("");

            Assert.IsNotNull(tagFacetQuery.TagGroups);
            Assert.IsNotNull(tagFacetQuery.TagGroups.Length == 1);
            Assert.AreEqual("", tagFacetQuery.TagGroups.Single());
        }

        [TestMethod]
        public void Empty_String_Array_Contructor()
        {
            var tagFacetQuery = new TagFacetQuery("", "tagGroup");

            Assert.IsNotNull(tagFacetQuery.TagGroups);
            Assert.IsNotNull(tagFacetQuery.TagGroups.Length == 2);
            Assert.AreEqual("", tagFacetQuery.TagGroups[0]);
            Assert.AreEqual("tagGroup", tagFacetQuery.TagGroups[1]);
        }

        [TestMethod]
        public void Different_Constructors_Object_Equals()
        {
            var obj1 = new TagFacetQuery();
            var obj2 = new TagFacetQuery("");

            Assert.AreEqual(obj1, obj2);
        }

    }
}
