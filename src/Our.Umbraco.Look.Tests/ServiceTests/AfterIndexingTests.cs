using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Our.Umbraco.Look.Tests.ServiceTests
{
    [TestClass]
    public class AfterIndexingTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] { });
        }

        [TestMethod]
        public void AfterIndexing_Called_Seven_Times()
        {
            var counter = 0;

            var afterIndexing = new Action<IndexingContext>(x => { counter++; });

            TestHelper.IndexThings(
                new [] {
                    new Thing(),
                    new Thing(),
                    new Thing(),
                    new Thing(),
                    new Thing(),
                    new Thing(),
                    new Thing()
                }, 
                null,
                afterIndexing);

            Assert.AreEqual(7, counter);
        }
    }
}
