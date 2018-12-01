using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Our.Umbraco.Look.Tests
{
    internal class TestHelper
    {
        internal static string DirectoryPath => Path.GetTempPath() + "LookTestData";


        internal static void GenerateTestData()
        {
            // generate a load of random test data, just to bulk it out
        }

        /// <summary>
        /// Add supplied collection into the test index
        /// </summary>
        /// <param name="things"></param>
        internal static void IndexThings(IEnumerable<Thing> things)
        {
            var nameStack = new Stack<string>(things.Select(x => x.Name));
            var dateStack = new Stack<DateTime?>(things.Select(x => x.Date));
            var textStack = new Stack<string>(things.Select(x => x.Text));
            var tagStack = new Stack<LookTag[]>(things.Select(x => x.Tags));
            var locationStack = new Stack<Location>(things.Select(x => x.Location));

            // setup indexers
            LookService.SetNameIndexer(x => nameStack.Pop());
            LookService.SetDateIndexer(x => dateStack.Pop());
            LookService.SetTextIndexer(x => textStack.Pop());
            LookService.SetTagIndexer(x => tagStack.Pop());
            LookService.SetLocationIndexer(x => locationStack.Pop());

            // null for IPublishedContent as not required
            var indexingContext = new IndexingContext(null, "TEST_CONTEXT");

            List<Document> documents = new List<Document>();

            foreach (var thing in things)
            {
                var document = new Document();

                LookService.Index(indexingContext, document);

                documents.Add(document);
            }

            // reset indexers
            LookService.SetNameIndexer(null);
            LookService.SetDateIndexer(null);
            LookService.SetTextIndexer(null);
            LookService.SetTagIndexer(null);
            LookService.SetLocationIndexer(null);

            TestHelper.IndexDocuments(documents);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        private static void IndexDocuments(IEnumerable<Document> documents)
        {
            var luceneDirectory = FSDirectory.Open(System.IO.Directory.CreateDirectory(TestHelper.DirectoryPath));
            var analyzer = new WhitespaceAnalyzer();

            var indexWriter = new IndexWriter(luceneDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (var document in documents)
            {
                indexWriter.AddDocument(document);
            }

            indexWriter.Optimize();

            indexWriter.Close();
        }

        ///// <summary>
        ///// Delete the test index from the file system
        ///// </summary>
        //internal static void DeleteIndex()
        //{
        //    System.IO.Directory.Delete(TestHelper.DirectoryPath, true);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">if supplied, allows test to use a specifc index</param>
        /// <returns></returns>
        internal static SearchingContext GetSearchingContext(string path = null)
        {
            if (path == null)
            {
                path = TestHelper.DirectoryPath; // use the default test index
            }

            return new SearchingContext() { 
                        Analyzer = new WhitespaceAnalyzer(),
                        EnableLeadingWildcards = true,
                        IndexSearcher = new IndexSearcher(new SimpleFSDirectory(new DirectoryInfo(path)), true)
            };
        }
    }

}
