using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System.Configuration;
using System.IO;

namespace Our.Umbraco.Look.Tests.DemoSiteTests
{
    [TestClass]
    public class NameQueryTests : BaseDemoSiteTests
    {   
        [TestMethod]
        public void Starts_With()
        {
            var lookResult = LookService.Query(
                                new LookQuery(this._searchingContext) {
                                        NameQuery = new NameQuery() {
                                            StartsWith = "T",
                                            CaseSensitive = true
                                        }
                                });

            Assert.IsTrue(lookResult.Success);
            Assert.IsTrue(lookResult.Total > 0);
        }

    }
}
