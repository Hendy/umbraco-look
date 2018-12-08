using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class NodeQueryConstructorTests
    {
        [TestMethod]
        public void Single_String_To_Array()
        {
            var nodeQuery1 = new NodeQuery("alias");
            var nodeQuery2 = new NodeQuery() { Aliases = new string[] { "alias" }  };

            Assert.AreEqual(nodeQuery1, nodeQuery2);
        }

        [TestMethod]
        public void Node_Type_And_Single_String()
        {
            var nodeQuery1 = new NodeQuery(PublishedItemType.Content, "alias");
            var nodeQuery2 = new NodeQuery()
            {
                Types = new PublishedItemType[] { PublishedItemType.Content },
                Aliases = new string[] { "alias" }
            };

            Assert.AreEqual(nodeQuery1, nodeQuery2);
        }
    }
}
