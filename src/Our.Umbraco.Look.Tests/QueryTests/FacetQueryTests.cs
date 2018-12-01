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
        private string _colour;

        private LookTag _red;
        private LookTag _orange;
        private LookTag _yellow;
        private LookTag _green;
        private LookTag _blue;
        private LookTag _indigo;
        private LookTag _violet;

        [TestInitialize]
        public void Initialize()
        {
            _colour = Guid.NewGuid().ToString("N"); // create new group of tags for these tests (rather than clear index - let it bulk out)

            _red = new LookTag(_colour, "red");
            _orange = new LookTag(_colour, "orange");
            _yellow = new LookTag(_colour, "yellow");
            _green = new LookTag(_colour, "green");
            _blue = new LookTag(_colour, "blue");
            _indigo = new LookTag(_colour, "indigo");
            _violet = new LookTag(_colour, "violet");

            TestHelper.IndexThings(new Thing[] {
                new Thing() { Tags = new LookTag[] { _red, _orange, _yellow, _green, _blue, _indigo, _violet } },
                new Thing() { Tags = new LookTag[] { _red, _orange, _yellow, _green, _blue, _indigo } },
                new Thing() { Tags = new LookTag[] { _red, _orange, _yellow, _green, _blue} },
                new Thing() { Tags = new LookTag[] { _red, _orange, _yellow, _green} },
                new Thing() { Tags = new LookTag[] { _red, _orange, _yellow } },
                new Thing() { Tags = new LookTag[] { _red, _orange } },
                new Thing() { Tags = new LookTag[] { _red } }
            });
        }

        [TestMethod]
        public void Facet_Counts()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery.All = new LookTag[] { _red };

            lookQuery.TagQuery.GetFacets = new string[] { _colour };

            var lookResult = LookService.Query(lookQuery);

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
            Assert.IsTrue(lookResult.Facets.Length == 7);
            Assert.IsTrue(lookResult.Facets.Single(x => _red.Equals(x.Tag)).Count == 7);
            Assert.IsTrue(lookResult.Facets.Single(x => _orange.Equals(x.Tag)).Count == 6);
            Assert.IsTrue(lookResult.Facets.Single(x => _yellow.Equals(x.Tag)).Count == 5);
            Assert.IsTrue(lookResult.Facets.Single(x => _green.Equals(x.Tag)).Count == 4);
            Assert.IsTrue(lookResult.Facets.Single(x => _blue.Equals(x.Tag)).Count == 3);
            Assert.IsTrue(lookResult.Facets.Single(x => _indigo.Equals(x.Tag)).Count == 2);
            Assert.IsTrue(lookResult.Facets.Single(x => _violet.Equals(x.Tag)).Count == 1);
        }

        [TestMethod]
        public void Apply_Facet_Then_Re_Query()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery.All = new LookTag[] { _red };

            lookQuery.TagQuery.GetFacets = new string[] { _colour };

            var lookResult = LookService.Query(lookQuery);

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total == 7);
            Assert.IsTrue(lookResult.Facets.Length == 7);

            var facet = lookResult.Facets.Single(x => _green.Equals(x.Tag));

            Assert.IsTrue(facet.Count == 4);

            lookQuery.ApplyFacet(facet);

            Assert.IsNull(lookQuery.Compiled);

            lookResult = LookService.Query(lookQuery);

            Assert.IsNotNull(lookQuery.Compiled);

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total == 4);
        }
    }
}
