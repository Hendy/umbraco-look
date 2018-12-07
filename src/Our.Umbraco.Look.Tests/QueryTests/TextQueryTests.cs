using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class TextQueryTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] {
                new Thing() { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam vitae." },
                new Thing() { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed hendrerit." },
                new Thing() { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec eget." },
                new Thing() { Text = "Maecenas pretium ipsum nec vestibulum pulvinar. Nulla ultrices fringilla mi." }
            });
        }

        [TestMethod]
        public void No_Highlighting()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TextQuery = new TextQuery("dolor");

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount > 0);
            Assert.IsTrue(string.IsNullOrWhiteSpace(((LookMatch)lookResult.First()).Highlight?.ToString()));
        }


        [TestMethod]
        public void Highlighting()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TextQuery = new TextQuery("dolor", true);

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(((LookMatch)lookResult.First()).Highlight.ToString()));
        }
    }
}
