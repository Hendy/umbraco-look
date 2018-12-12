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
                new Thing() { Date = new DateTime(2000, 1, 2) },
                new Thing() { Date = new DateTime(2000, 1, 3) },
                new Thing() { Date = new DateTime(2000, 1, 4) },
                new Thing() { Date = new DateTime(2000, 1, 5) },
                new Thing() { Date = new DateTime(2000, 1, 6) },
                new Thing() { Date = new DateTime(2000, 1, 7) },
                new Thing() { Date = new DateTime(2000, 1, 8) },
                new Thing() { Date = new DateTime(2000, 1, 9) },
                new Thing() { Date = new DateTime(2000, 1, 10) },
                new Thing() { Date = new DateTime(2005, 02, 16) },
                new Thing() { Date = DateTime.Now },
                new Thing() { Date = DateTime.MaxValue }
            });
        }

        [TestMethod]
        public void Boundary_Inclusive()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery = new DateQuery();
            lookQuery.DateQuery.After = new DateTime(2000, 1, 1);
            lookQuery.DateQuery.Before = new DateTime(2000, 1, 3);
            lookQuery.DateQuery.Boundary = DateBoundary.Inclusive;

            Assert.AreEqual(3, lookQuery.Run().TotalItemCount);
        }

        [TestMethod]
        public void Boundary_Exclusive()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery = new DateQuery();
            lookQuery.DateQuery.After = new DateTime(2000, 1, 10);
            lookQuery.DateQuery.Boundary = DateBoundary.Exclusive;

            Assert.AreEqual(3, lookQuery.Run().TotalItemCount);
        }

        [TestMethod]
        public void Boundary_Inclusive_Exclusive()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery = new DateQuery();
            lookQuery.DateQuery.Before = new DateTime(2000, 1, 1);
            lookQuery.DateQuery.Boundary = DateBoundary.BeforeInclusiveAfterExclusive;

            Assert.AreEqual(2, lookQuery.Run().TotalItemCount);
        }

        [TestMethod]
        public void Boundary_Exclusive_Inclusive()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery = new DateQuery();
            lookQuery.DateQuery.Before = new DateTime(2000, 1, 1);
            lookQuery.DateQuery.Boundary = DateBoundary.BeforeExclusiveAfterInclusive;

            Assert.AreEqual(1, lookQuery.Run().TotalItemCount);
        }

    }
}
