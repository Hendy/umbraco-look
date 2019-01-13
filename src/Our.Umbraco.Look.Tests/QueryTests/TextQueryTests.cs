using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void Has_Text()
        {
            Assert.AreEqual(4, new LookQuery(TestHelper.GetSearchingContext()) { TextQuery = new TextQuery() }.Search().TotalItemCount);
        }

        [TestMethod]
        public void No_Highlighting()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TextQuery = new TextQuery("dolor");

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount > 0);
            Assert.IsTrue(string.IsNullOrWhiteSpace(lookResult.Matches.First().Highlight?.ToString()));
        }


        [TestMethod]
        public void Highlighting()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.TextQuery = new TextQuery("dolor", true);

            var lookResult = lookQuery.Search();

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.TotalItemCount > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(lookResult.Matches.First().Highlight.ToString()));
        }
    }
}
