using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class LookTagConstructorTests
    {
        [TestMethod]
        public void Tag_In_Default_Group()
        {
            var lookTag = new LookTag("tag");

            Assert.AreEqual(string.Empty, lookTag.Group);
            Assert.AreEqual("tag", lookTag.Name);
        }

        [TestMethod]
        public void Tag_In_Default_Group_Using_Delimiter()
        {
            var lookTag = new LookTag(":tag");

            Assert.AreEqual(string.Empty, lookTag.Group);
            Assert.AreEqual("tag", lookTag.Name);
        }

        [TestMethod]
        public void Tag_Containing_Delimiter_In_Default_Group_Using_Delimiter()
        {
            var lookTag = new LookTag(":tag:with:delimiter");

            Assert.AreEqual(string.Empty, lookTag.Group);
            Assert.AreEqual("tag:with:delimiter", lookTag.Name);
        }

        [TestMethod]
        public void Tag_Containing_Delimiter_In_Named_Group()
        {
            var lookTag = new LookTag("group:tag:with:delimiter");

            Assert.AreEqual("group", lookTag.Group);
            Assert.AreEqual("tag:with:delimiter", lookTag.Name);
        }

        [TestMethod]
        public void Tag_In_Named_Group()
        {
            var lookTag = new LookTag("group:tag");

            Assert.AreEqual("group", lookTag.Group);
            Assert.AreEqual("tag", lookTag.Name);
        }

        [TestMethod]
        public void Tag_In_Named_Group_Overloaded_Constructor()
        {
            var lookTag = new LookTag("group", "tag");

            Assert.AreEqual("group", lookTag.Group);
            Assert.AreEqual("tag", lookTag.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Invalid_Char_In_Group_Name()
        {
            var lookTag = new LookTag("*", "tag");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Group_Name_Too_Long()
        {
            var lookTag = new LookTag("123456789012345678901234567890123456789012345678901", "tag");
        }
    }
}
