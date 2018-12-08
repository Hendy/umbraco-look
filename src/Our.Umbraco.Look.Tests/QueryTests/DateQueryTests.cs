using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
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

            lookQuery.DateQuery.After = new DateTime(2000, 1, 10);
            lookQuery.DateQuery.Boundary = DateBoundary.Inclusive;

            Assert.AreEqual(4, lookQuery.Query().TotalItemCount);
        }

        [TestMethod]
        public void Boundary_Exclusive()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery.After = new DateTime(2000, 1, 10);
            lookQuery.DateQuery.Boundary = DateBoundary.Exclusive;

            Assert.AreEqual(3, lookQuery.Query().TotalItemCount);
        }

        [TestMethod]
        public void Boundary_After_Exclusive_Before_Inclusive()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery.Before = new DateTime(2000, 1, 1);
            lookQuery.DateQuery.Boundary = DateBoundary.AfterExclusiveBeforeInclusive;

            Assert.AreEqual(2, lookQuery.Query().TotalItemCount);
        }

        [TestMethod]
        public void Boundary_After_Inclusive_Before_Exclusive()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.DateQuery.Before = new DateTime(2000, 1, 1);
            lookQuery.DateQuery.Boundary = DateBoundary.AfterInclusiveBeforeExclusive;

            Assert.AreEqual(1, lookQuery.Query().TotalItemCount);
        }
    }
}
