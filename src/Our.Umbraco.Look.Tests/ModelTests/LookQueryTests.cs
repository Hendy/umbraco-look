using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Our.Umbraco.Look.Tests.ModelTests
{
    [TestClass]
    public class LookQueryTests
    {
        [TestMethod]
        public void Clone_Look_Query()
        {
            Assert.AreEqual(new LookQuery(), new LookQuery().Clone());
        }
    }
}
