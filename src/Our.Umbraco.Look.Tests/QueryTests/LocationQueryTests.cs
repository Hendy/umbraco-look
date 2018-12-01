using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System;
using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class LocationQueryTests
    {
        private string _name; 

        private Location _umbracoHQ = new Location(55.406330, 10.388500);
        private Location _copenhagen = new Location(55.6761, 12.5683);
        private Location _london = new Location(51.5074, 0.1278);
        private Location _paris = new Location(48.8566, 2.3522);
        private Location _newYork = new Location(40.7128, -74.0060);

        [TestInitialize]
        public void Initialize()
        {
            _name = Guid.NewGuid().ToString("N");

            TestHelper.IndexThings(new Thing[]
            {
                new Thing() { Name = _name, Location = _copenhagen },
                new Thing() { Name = _name, Location = _london },
                new Thing() { Name = _name, Location = _paris },
                new Thing() { Name = _name, Location = _newYork },
            });
        }

        [TestMethod]
        public void Distance_Sorting()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery.Is = _name;
            lookQuery.LocationQuery.Location = _london;

            lookQuery.SortOn = SortOn.Distance;

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total == 4);

            var locations = lookResult.Select(x => x.Location).ToArray();

            Assert.IsTrue(locations[0].Equals(_london));
            Assert.IsTrue(locations[1].Equals(_paris));
            Assert.IsTrue(locations[2].Equals(_copenhagen));
            Assert.IsTrue(locations[3].Equals(_newYork));
        }

        [TestMethod]
        public void Max_Distance()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.NameQuery.Is = _name;
            lookQuery.LocationQuery.Location = _london;
            lookQuery.LocationQuery.MaxDistance = new Distance(300, DistanceUnit.Miles);

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total == 2);

            Assert.IsTrue(lookResult.Any(x => x.Location.Equals(_london)));
            Assert.IsTrue(lookResult.Any(x => x.Location.Equals(_paris)));
        }
    }
}
