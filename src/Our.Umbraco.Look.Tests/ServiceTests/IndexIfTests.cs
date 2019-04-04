using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Look.Tests.ServiceTests
{
    [TestClass]
    public class ServiceTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] { });
        }

        [TestMethod]
        public void Index_Last_Item_Only()
        {            
            var indexIf = new Queue<bool>(new[] { false, false, true }); // set of indexIf responses

            var tag = new LookTag(Guid.NewGuid().ToString("N"));

            var lookQuery = new LookQuery(TestHelper.GetSearchingContext()) { TagQuery = new TagQuery() { Has = tag }  };

            Assert.IsTrue(lookQuery.Search().TotalItemCount == 0);

            var tags = new[] { tag };

            TestHelper.IndexThings(
                new [] {
                    new Thing() { Name = "First", Tags = tags },
                    new Thing() { Name = "Second", Tags = tags },
                    new Thing() { Name = "Third", Tags = tags }
                }, 
                x => indexIf.Dequeue());

            lookQuery.SearchingContext = TestHelper.GetSearchingContext(); // reset the context

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.TotalItemCount == 1);
            Assert.AreEqual("Third", lookResult.Matches.First().Name);
        }
    }
}
