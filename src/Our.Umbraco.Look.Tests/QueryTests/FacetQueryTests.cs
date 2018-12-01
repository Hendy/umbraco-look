using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System;
using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class FacetQueryTests
    {
        //[ClassInitialize]
        //public static void ClassInitialize(TestContext testContext)
        //{
        //}

        [TestMethod]
        public void Facet_Counts()
        {
            var colour = Guid.NewGuid().ToString("N");

            var red = new LookTag(colour, "red");
            var green = new LookTag(colour, "green");
            var blue = new LookTag(colour, "blue");

            TestHelper.IndexThings(new Thing[] {
                new Thing() { Tags = new LookTag[] { red, green, blue } },
                new Thing() { Tags = new LookTag[] { red, green, blue } },
                new Thing() { Tags = new LookTag[] { red, green } },
                new Thing() { Tags = new LookTag[] { red, green } },
                new Thing() { Tags = new LookTag[] { red } },
                new Thing() { Tags = new LookTag[] { red } }
            });

            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery.All = new LookTag[] { red };
            lookQuery.TagQuery.GetFacets = new string[] { colour };

            var lookResult = LookService.Query(lookQuery);

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
            Assert.IsTrue(lookResult.Facets.Length == 3);
            Assert.IsTrue(lookResult.Facets.Single(x => red.Equals(x.Tag)).Count == 6);
            Assert.IsTrue(lookResult.Facets.Single(x => green.Equals(x.Tag)).Count == 4);
            Assert.IsTrue(lookResult.Facets.Single(x => blue.Equals(x.Tag)).Count == 2);
        }


    }
}
