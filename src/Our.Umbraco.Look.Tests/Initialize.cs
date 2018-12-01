using Microsoft.VisualStudio.TestTools.UnitTesting;
using Our.Umbraco.Look.Services;

namespace Our.Umbraco.Look.Tests
{
    [TestClass]
    public static class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            // Wire up the location indexers
            LookService.Initialize();
        }    
    }
}
