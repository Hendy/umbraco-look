using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Our.Umbraco.Look.Tests.ServiceTests
{
    [TestClass]
    public class ServiceTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.IndexThings(new Thing[] { });
        }

        [TestMethod]
        public void Index_Last_Item_Only()
        {            
            var stack = new Stack<bool>(new[] { true, false, false }); // set of indexIf responses

            var key = Guid.NewGuid().ToString("N");

            var lookQuery = new LookQuery(TestHelper.GetSearchingContext()) { NameQuery = new NameQuery() { Is = key } };

            Assert.IsTrue(lookQuery.Search().TotalItemCount == 0);

            TestHelper.IndexThings(
                new [] {
                    new Thing() { Name = key },
                    new Thing() { Name = key },
                    new Thing() { Name = key }
                }, 
                x => stack.Pop());

            lookQuery.SearchingContext = TestHelper.GetSearchingContext(); // reset the context

            Assert.IsTrue(lookQuery.Search().TotalItemCount == 1);
        }
    }
}
