using Lucene.Net.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Our.Umbraco.Look.Tests.QueryTests
{
    [TestClass]
    public class RequestFieldsTests
    {
        private const string TEST_FIELD = "DE2494AE_FBF5_42A4_94C4_373DE3FC98B1";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var document = new Document();

            var field = new Field(
                                TEST_FIELD,
                                "Not_Look_Value",
                                Field.Store.YES,
                                Field.Index.NOT_ANALYZED,
                                Field.TermVector.NO);

            document.Add(field);

            TestHelper.IndexDocuments(new[] { document });
        }

        [TestMethod]
        public void Look_Fields_Only()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.RequestFields = RequestFields.LookFieldsOnly;
            lookQuery.RawQuery = $"+{TEST_FIELD}:Not_Look_Value";

            var lookResult = lookQuery.Run();

            Assert.AreEqual(1, lookResult.TotalItemCount);
            Assert.IsFalse(lookResult.Single().Fields.ContainsKey(TEST_FIELD));            
        }

        [TestMethod]
        public void All_Fields()
        {
            var lookQuery = new LookQuery(TestHelper.GetSearchingContext());

            lookQuery.RequestFields = RequestFields.AllFields;
            lookQuery.RawQuery = $"+{TEST_FIELD}:Not_Look_Value";

            var lookResult = lookQuery.Run();

            Assert.AreEqual(1, lookResult.TotalItemCount);
            Assert.IsTrue(lookResult.Single().Fields.ContainsKey(TEST_FIELD));
        }
    }
}
