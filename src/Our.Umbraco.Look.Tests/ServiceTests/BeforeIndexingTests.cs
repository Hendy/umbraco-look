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
        public void BeforeIndexing_CancelFirstTwo()
        {
            var searchingContext = TestHelper.GetSearchingContext();

            var beforeIndexing = new Queue<Action<IndexingContext>>(
                    new Action<IndexingContext>[] {
                        new Action<IndexingContext>(x => { x.Cancel(); }),
                        new Action<IndexingContext>(x => { x.Cancel(); }),
                        new Action<IndexingContext>(x => { })
                    });


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
                x => beforeIndexing.Dequeue());

            lookQuery.SearchingContext = TestHelper.GetSearchingContext(); // reset the context

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.TotalItemCount == 1);
            Assert.AreEqual("Third", lookResult.Matches.First().Name);
        }
    }
}
