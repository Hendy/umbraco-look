using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;

namespace Our.Umbraco.Look.Tests
{
    [TestClass]
    public class LookTagConstructorTests
    {
        [TestMethod]
        public void Construct_Tag_In_Default_Group()
        {
            var lookTag = new LookTag("tag");

            Assert.AreEqual(string.Empty, lookTag.Group);
            Assert.AreEqual("tag", lookTag.Name);
        }

        [TestMethod]
        public void Construct_Tag_In_Default_Group_Using_Delimiter()
        {
            var lookTag = new LookTag(":tag");

            Assert.AreEqual(string.Empty, lookTag.Group);
            Assert.AreEqual("tag", lookTag.Name);
        }

        [TestMethod]
        public void Construct_Tag_Containing_Delimiter_In_Default_Group()
        {
            var lookTag = new LookTag(":tag:with:delimiter");

            Assert.AreEqual(string.Empty, lookTag.Group);
            Assert.AreEqual("tag:with:delimiter", lookTag.Name);
        }

        [TestMethod]
        public void Construct_Tag_In_Named_Group()
        {
            var lookTag = new LookTag("group:tag");

            Assert.AreEqual("group", lookTag.Group);
            Assert.AreEqual("tag", lookTag.Name);
        }
    }
}
