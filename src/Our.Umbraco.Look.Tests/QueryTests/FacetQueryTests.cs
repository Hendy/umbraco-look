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
            var orange = new LookTag(colour, "orange");
            var yellow = new LookTag(colour, "yellow");
            var green = new LookTag(colour, "green");
            var blue = new LookTag(colour, "blue");
            var indigo = new LookTag(colour, "indigo");
            var violet = new LookTag(colour, "violet");

            TestHelper.IndexThings(new Thing[] {
                new Thing() { Tags = new LookTag[] { red, orange, yellow, green, blue, indigo, violet } },
                new Thing() { Tags = new LookTag[] { red, orange, yellow, green, blue, indigo } },
                new Thing() { Tags = new LookTag[] { red, orange, yellow, green, blue} },
                new Thing() { Tags = new LookTag[] { red, orange, yellow, green} },
                new Thing() { Tags = new LookTag[] { red, orange, yellow } },
                new Thing() { Tags = new LookTag[] { red, orange } },
                new Thing() { Tags = new LookTag[] { red } }
            });

            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery.All = new LookTag[] { red };
            lookQuery.TagQuery.GetFacets = new string[] { colour };

            var lookResult = LookService.Query(lookQuery);

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
            Assert.IsTrue(lookResult.Facets.Length == 7);
            Assert.IsTrue(lookResult.Facets.Single(x => red.Equals(x.Tag)).Count == 7);
            Assert.IsTrue(lookResult.Facets.Single(x => orange.Equals(x.Tag)).Count == 6);
            Assert.IsTrue(lookResult.Facets.Single(x => yellow.Equals(x.Tag)).Count == 5);
            Assert.IsTrue(lookResult.Facets.Single(x => green.Equals(x.Tag)).Count == 4);
            Assert.IsTrue(lookResult.Facets.Single(x => blue.Equals(x.Tag)).Count == 3);
            Assert.IsTrue(lookResult.Facets.Single(x => indigo.Equals(x.Tag)).Count == 2);
            Assert.IsTrue(lookResult.Facets.Single(x => violet.Equals(x.Tag)).Count == 1);
        }

        [TestMethod]
        public void Bl()
        {

        }

    }
}
