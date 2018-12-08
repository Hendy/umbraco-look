using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class LocationQueryTests
    {
        private readonly static Location _umbracoHQ = new Location(55.406330, 10.388500);
        private readonly static Location _copenhagen = new Location(55.6761, 12.5683);
        private readonly static Location _london = new Location(51.5074, 0.1278);
        private readonly static Location _paris = new Location(48.8566, 2.3522);
        private readonly static Location _newYork = new Location(40.7128, -74.0060);

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] {
                new Thing() { Location = _copenhagen },
                new Thing() { Location = _london },
                new Thing() { Location = _paris },
                new Thing() { Location = _newYork },
            });
        }

        [TestMethod]
        public void Distance_Sorting()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());
            
            //lookQuery.LocationQuery.HasLocation = true; // indicate any items with a location can be returned
            lookQuery.LocationQuery.Location = _london;

            lookQuery.SortOn = SortOn.Distance;

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 4);

            var locations = lookResult.Matches.Select(x => x.Location).ToArray();

            Assert.IsTrue(locations[0].Equals(_london));
            Assert.IsTrue(locations[1].Equals(_paris));
            Assert.IsTrue(locations[2].Equals(_copenhagen));
            Assert.IsTrue(locations[3].Equals(_newYork));
        }

        [TestMethod]
        public void Max_Distance()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.LocationQuery.Location = _london;
            lookQuery.LocationQuery.MaxDistance = new Distance(300, DistanceUnit.Miles);

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 2);

            Assert.IsTrue(lookResult.Matches.Any(x => x.Location.Equals(_london)));
            Assert.IsTrue(lookResult.Matches.Any(x => x.Location.Equals(_paris)));
        }
    }
}
