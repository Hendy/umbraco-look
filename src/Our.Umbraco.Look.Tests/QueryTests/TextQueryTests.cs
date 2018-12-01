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
                new Thing() { Name = "name 1", Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam vitae." },
                new Thing() { Name = "name 2", Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed hendrerit." },
                new Thing() { Name = "name 3", Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec eget." },
                new Thing() { Name = "name 4", Text = "Maecenas pretium ipsum nec vestibulum pulvinar. Nulla ultrices fringilla mi." }
            });
        }

        [TestMethod]
        public void Highlighting()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TextQuery = new TextQuery("dolor", true);

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(lookResult.First().Highlight.ToString()));
        }
    }
}
