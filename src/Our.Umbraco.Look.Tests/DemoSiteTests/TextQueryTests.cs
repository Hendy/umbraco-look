using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System.Linq;

namespace Our.Umbraco.Look.Tests.DemoSiteTests
{
    [TestClass]
    public class TextQueryTests : BaseDemoSiteTests
    {   
        [TestMethod]
        public void Things_With_Base_Highlighting()
        {
            var lookQuery = new LookQuery(this._searchingContext);

            lookQuery.TextQuery = new TextQuery() { SearchText = "base", GetHighlight = true, GetText = false };

            var lookResult = LookService.Query(lookQuery);

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(lookResult.First().Highlight.ToString()));
        }
    }
}
