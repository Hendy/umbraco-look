using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Our.Umbraco.Look.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class GetFlatDetachedDescendants
    {
        [TestMethod]
        public void Null_IPublishedContent()
        {
            IPublishedContent content = null;

            var result = content.GetFlatDetachedDescendants();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void No_Properties()
        {
            var content = new Mock<IPublishedContent>();

            content.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>());

            var result = content.Object.GetFlatDetachedDescendants();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Empty_Collection_Property()
        {
            var content = new Mock<IPublishedContent>();
            var property = new Mock<IPublishedProperty>();

            property.SetupGet(x => x.Value).Returns(Enumerable.Empty<IPublishedContent>());

            content.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property.Object });

            var result = content.Object.GetFlatDetachedDescendants();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Null_Populated_Collection_Property()
        {
            var content = new Mock<IPublishedContent>();
            var property = new Mock<IPublishedProperty>();

            property.SetupGet(x => x.Value).Returns(new IPublishedContent[] { null, null });

            content.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property.Object });

            var result = content.Object.GetFlatDetachedDescendants();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Populated_Collection_Property()
        {
            var content = new Mock<IPublishedContent>();
            var property = new Mock<IPublishedProperty>();
            var detached = new Mock<IPublishedContentWithKey>();

            detached.SetupGet(x => x.Id).Returns(0);
            detached.SetupGet(x => x.Key).Returns(Guid.NewGuid());

            detached.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>());

            property.SetupGet(x => x.Value).Returns(new IPublishedContent[] { detached.Object });

            content.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property.Object });

            var result = content.Object.GetFlatDetachedDescendants();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Deeply_Populated_Collection_Property()
        {
            var content = new Mock<IPublishedContent>();

            var detached1 = new Mock<IPublishedContentWithKey>();
            var detached2 = new Mock<IPublishedContentWithKey>();
            var detached3 = new Mock<IPublishedContentWithKey>();

            var property1 = new Mock<IPublishedProperty>();
            var property2 = new Mock<IPublishedProperty>();
            var property3 = new Mock<IPublishedProperty>();
            var property4 = new Mock<IPublishedProperty>();

            content.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property1.Object });

            detached1.SetupGet(x => x.Id).Returns(0);
            detached2.SetupGet(x => x.Id).Returns(0);
            detached3.SetupGet(x => x.Id).Returns(0);

            detached1.SetupGet(x => x.Key).Returns(Guid.NewGuid());
            detached2.SetupGet(x => x.Key).Returns(Guid.NewGuid());
            detached3.SetupGet(x => x.Key).Returns(Guid.NewGuid());

            detached1.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property2.Object });
            detached2.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property3.Object });
            detached3.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property4.Object });

            property1.SetupGet(x => x.Value).Returns(new IPublishedContent[] { detached1.Object });
            property2.SetupGet(x => x.Value).Returns(new IPublishedContent[] { detached2.Object });
            property3.SetupGet(x => x.Value).Returns(new IPublishedContent[] { detached3.Object });
            property4.SetupGet(x => x.Value).Returns(new List<IPublishedProperty>());

            var result = content.Object.GetFlatDetachedDescendants();

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        //[TestMethod]
        //public void Infinite_Loop_Populated_Collection_Property()
        //{
        //    var content = new Mock<IPublishedContent>();
        //    var detached = new Mock<IPublishedContent>();
        //    var property = new Mock<IPublishedProperty>();

        //    content.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property.Object });

        //    detached.SetupGet(x => x.Id).Returns(0);
        //    detached.SetupGet(x => x.Properties).Returns(new List<IPublishedProperty>() { property.Object });

        //    property.SetupGet(x => x.Value).Returns(new IPublishedContent[] { detached.Object });

        //    var result = content.Object.GetFlatDetachedDescendants();

        //    Assert.IsNotNull(result);
        //}
    }
}
