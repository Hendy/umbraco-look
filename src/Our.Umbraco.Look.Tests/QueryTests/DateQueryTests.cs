using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class DateQueryTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] {
                new Thing() { Date = DateTime.MinValue },
                new Thing() { Date = new DateTime(2000, 1, 1) },
                new Thing() { Date = new DateTime(2005, 02, 16) },
                new Thing() { Date = DateTime.Now },
                new Thing() { Date = DateTime.MaxValue }
            });
        }

        [TestMethod]
        public void Before()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery.Before = new DateTime(2000, 1, 2);

            Assert.AreEqual(2, lookQuery.Query().TotalItemCount);
        }
    }
}
