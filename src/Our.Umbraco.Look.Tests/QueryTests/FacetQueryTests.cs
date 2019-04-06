﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class FacetQueryTests
    {
        private static string _colour = "colour";

        private static LookTag _red = new LookTag(_colour, "red");
        private static LookTag _orange = new LookTag(_colour, "orange");
        private static LookTag _yellow = new LookTag(_colour, "yellow");
        private static LookTag _green = new LookTag(_colour, "green");
        private static LookTag _blue = new LookTag(_colour, "blue");
        private static LookTag _indigo = new LookTag(_colour, "indigo");
        private static LookTag _violet = new LookTag(_colour, "violet");

        /// <summary>
        /// Index all colour tags
        /// </summary>
        /// <param name="testContext"></param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
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

            lookQuery.TagQuery = new TagQuery();
            lookQuery.TagQuery.HasAll = new LookTag[] { _red };
            lookQuery.TagQuery.FacetOn = new TagFacetQuery(_colour);

            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount > 0);
            Assert.IsTrue(lookResult.Facets.Length == 7);
            Assert.IsTrue(lookResult.Facets.Single(x => _red.Equals(x.Tags.Single())).Count == 7);
            Assert.IsTrue(lookResult.Facets.Single(x => _orange.Equals(x.Tags.Single())).Count == 6);
            Assert.IsTrue(lookResult.Facets.Single(x => _yellow.Equals(x.Tags.Single())).Count == 5);
            Assert.IsTrue(lookResult.Facets.Single(x => _green.Equals(x.Tags.Single())).Count == 4);
            Assert.IsTrue(lookResult.Facets.Single(x => _blue.Equals(x.Tags.Single())).Count == 3);
            Assert.IsTrue(lookResult.Facets.Single(x => _indigo.Equals(x.Tags.Single())).Count == 2);
            Assert.IsTrue(lookResult.Facets.Single(x => _violet.Equals(x.Tags.Single())).Count == 1);
        }

        [TestMethod]
        public void Query_Apply_Random_Facet_Then_Re_Query()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery();
            lookQuery.TagQuery.HasAll = new LookTag[] { _red };
            lookQuery.TagQuery.FacetOn = new TagFacetQuery(_colour);

            // first query
            var lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount == 7);
            Assert.IsTrue(lookResult.Facets.Length == 7);

            // pick a random facet
            var random = new Random();
            var facet = lookResult
                            .Facets
                            .OrderBy(x => random.Next())
                            .First();

            // get the expected count
            var facetCount = facet.Count;

            // apply facet to query
            lookQuery.ApplyFacet(facet);

            // second query
            lookResult = lookQuery.Search();

            Assert.IsNotNull(lookResult);
            Assert.IsTrue(lookResult.Success);
            Assert.AreEqual(facetCount, lookResult.TotalItemCount);
        }

        [TestMethod]
        public void Apply_Facets_In_Turn()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TagQuery = new TagQuery();
            lookQuery.TagQuery.HasAll = new LookTag[] { _red };
            lookQuery.TagQuery.FacetOn = new TagFacetQuery(_colour);

            foreach (var facet in lookQuery.Search().Facets)
            {
                // clone the lookQuery (else all facets would be added together)
                var result = lookQuery.Clone().ApplyFacet(facet).Search(); 

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Success);
                Assert.AreEqual(facet.Count, result.TotalItemCount);
            }
        }
    }
}
