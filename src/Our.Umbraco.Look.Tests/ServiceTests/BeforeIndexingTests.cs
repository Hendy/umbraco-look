using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Our.Umbraco.Look.Tests.ServiceTests
{
    [TestClass]
    public class BeforeIndexingTests
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

            var counter = 0;
            var beforeIndexing = new Action<IndexingContext>(
                        x => {
                            counter++;
                            if (counter != 3) { x.Cancel(); }                            
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
                beforeIndexing);

            lookQuery.SearchingContext = TestHelper.GetSearchingContext(); // reset the context (to take into account new things indexed)

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.TotalItemCount == 1);
            Assert.AreEqual("Third", lookResult.Matches.First().Name);
        }
    }
}
