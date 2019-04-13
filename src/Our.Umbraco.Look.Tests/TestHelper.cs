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
        private static string DirectoryPath => Path.GetTempPath() + "LookTestData";

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

            return new SearchingContext()
            {
                Analyzer = new WhitespaceAnalyzer(),
                EnableLeadingWildcards = true,
                IndexSearcher = new IndexSearcher(new SimpleFSDirectory(new DirectoryInfo(path)), true)
            };
        }

        /// <summary>
        /// Index the supplied things
        /// </summary>
        /// <param name="things">The things to add into the index</param>
        /// <param name="beforeIndexing">optional action to call before indexing each thing</param>
        /// <param name="afterIndexing">optional acton to call after indexing each thing</param>
        internal static void IndexThings(IEnumerable<Thing> things, Action<IndexingContext> beforeIndexing = null, Action<IndexingContext> afterIndexing = null)
        {
            var nameStack = new Stack<string>(things.Select(x => x.Name));
            var dateStack = new Stack<DateTime?>(things.Select(x => x.Date));
            var textStack = new Stack<string>(things.Select(x => x.Text));
            var tagStack = new Stack<LookTag[]>(things.Select(x => x.Tags));
            var locationStack = new Stack<Location>(things.Select(x => x.Location));

            // use supplied, or do nothing
            LookConfiguration.BeforeIndexing = beforeIndexing;
            LookConfiguration.AfterIndexing = afterIndexing;

            // setup indexers
            LookConfiguration.NameIndexer = x => nameStack.Pop();
            LookConfiguration.DateIndexer = x => dateStack.Pop();
            LookConfiguration.TextIndexer = x => textStack.Pop();
            LookConfiguration.TagIndexer = x => tagStack.Pop();
            LookConfiguration.LocationIndexer = x => locationStack.Pop();

            List<Document> documents = new List<Document>();

            foreach (var thing in things)
            {
                var document = new Document();
                
                var indexingContext = new IndexingContext(null, null, "UnitTestIndexer"); // null for IPublishedContent as not required

                LookService.Index(indexingContext, document);

                if (!indexingContext.Cancelled)
                {
                    documents.Add(document);
                }                
            }

            //reset
            LookConfiguration.BeforeIndexing = null;
            LookConfiguration.AfterIndexing = null;

            LookConfiguration.NameIndexer = null;
            LookConfiguration.DateIndexer = null;
            LookConfiguration.TextIndexer = null;
            LookConfiguration.TagIndexer = null;
            LookConfiguration.LocationIndexer = null;

            TestHelper.IndexDocuments(documents);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        internal static void IndexDocuments(IEnumerable<Document> documents)
        {
            var luceneDirectory = FSDirectory.Open(System.IO.Directory.CreateDirectory(TestHelper.DirectoryPath));
            var analyzer = new WhitespaceAnalyzer();

            var indexWriter = new IndexWriter(luceneDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (var document in documents)
            {
                indexWriter.AddDocument(document);
            }

            indexWriter.Optimize();

            indexWriter.Commit();
            
            indexWriter.Close();
        }

        /// <summary>
        /// Delete the test index from the file system
        /// </summary>
        internal static void DeleteIndex()
        {
            if (System.IO.Directory.Exists(TestHelper.DirectoryPath))
            {
                System.IO.Directory.Delete(TestHelper.DirectoryPath, true);
            }
        }
    }
}
