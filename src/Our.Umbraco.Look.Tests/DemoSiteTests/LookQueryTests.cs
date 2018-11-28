using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;

namespace Our.Umbraco.Look.Tests.DemoSiteTests
{
    [TestClass]
    public class LookQueryTests : BaseDemoSiteTests
    {
        [TestMethod]
        public void Empty_Query()
        {
            var lookQuery = new LookQuery(this._searchingContext);

            var lookResult = LookService.Query(lookQuery);

            Assert.IsNotNull(lookResult);
            Assert.IsFalse(lookResult.Success);
            Assert.IsTrue(lookResult.Total == 0);
        }

    }
}
