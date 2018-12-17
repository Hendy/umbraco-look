using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Our.Umbraco.Look.Tests
{
    [TestClass]
    public static class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            // always start with an empty index
            TestHelper.DeleteIndex();

            // Wire up the location indexers
            LookService.Initialize(null);
        }    
    }
}
