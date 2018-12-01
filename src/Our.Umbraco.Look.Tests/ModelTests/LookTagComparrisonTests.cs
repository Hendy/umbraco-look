using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class LookTagComparrisonTests
    {
        [TestMethod]
        public void Same_Tag()
        {
            var lookTag1 = new LookTag("group:tag");
            var lookTag2 = new LookTag("group:tag");

            Assert.AreEqual(lookTag1, lookTag2);
        }

        [TestMethod]
        public void Same_Tag_Different_Constructors()
        {
            var lookTag1 = new LookTag("group:tag");
            var lookTag2 = new LookTag("group", "tag");

            Assert.AreEqual(lookTag1, lookTag2);
        }

        [TestMethod]
        public void Different_Tags()
        {
            var lookTag1 = new LookTag("tag");
            var lookTag2 = new LookTag("group", "tag");

            Assert.AreNotEqual(lookTag1, lookTag2);
        }
    }
}
