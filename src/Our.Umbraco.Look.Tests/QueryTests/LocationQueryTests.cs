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
                new Thing() { Name = _name, Location = _umbracoHQ },
                new Thing() { Name = _name, Location = _copenhagen },
                new Thing() { Name = _name, Location = _london },
                new Thing() { Name = _name, Location = _paris },
                new Thing() { Name = _name, Location = _newYork },
            });
        }

        //[TestMethod]
        //public void Distance_Calculations()
        //{
        //    var lookQuery = new LookQuery(TestHelper.GetSearchingContext());
            
        //    lookQuery.NameQuery.Is = _name;
        //    lookQuery.LocationQuery.Location = _paris;
            
        //    //lookQuery.LocationQuery.MaxDistance = 1000
        //    //lookQuery.SortOn = SortOn.Distance;

        //    var lookResult = LookService.Query(lookQuery);

        //    Assert.IsTrue(lookResult.Success);
        //    Assert.IsTrue(lookResult.Total > 0);
        //    //Assert.IsTrue(lookResult.First().Location.Equals(_umbracoHQ));
        //    //Assert.IsTrue(lookResult.Last().Location.Equals(_newYork));
        //}
    }
}
