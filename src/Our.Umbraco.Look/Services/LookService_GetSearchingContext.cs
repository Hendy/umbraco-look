using Examine;
using Lucene.Net.Search;
using Umbraco.Core.Logging;
using UmbracoExamine;
using Examine.LuceneEngine.Providers;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Get the configuration details of an Exmaine searcher, so Lucene can be queried in the same way,
        /// consumer needs to know Lucene directory, the analyser (for the text field) and whether to use wildcards)
        /// TODO: move validation logic out into initialize, so quicker to get content during a query
        /// </summary>
        /// <param name="searcherName">The name of the Examine seracher (see ExamineSettings.config)</param>
        /// <returns>SearchingContext if found, otherwise null</returns>
        private static SearchingContext GetSearchingContext(string searcherName)
        {
            var searcher = !string.IsNullOrWhiteSpace(searcherName) 
                            ? ExamineManager.Instance.SearchProviderCollection[searcherName]
                            : ExamineManager.Instance.DefaultSearchProvider;

            if (searcher == null)
            {
                LogHelper.Debug(typeof(LookService), $"Unable to find Examine searcher '{ searcherName }'");
            }
            else
            {
                if (!(searcher is LuceneSearcher))
                {
                    LogHelper.Debug(typeof(LookService), $"Examine searcher of unexpected type '{ searcher.GetType() }'");
                }
                else
                {
                    var luceneSearcher = (LuceneSearcher)searcher;

                    var indexSetDirectory = LookService.Instance.IndexSetDirectories[luceneSearcher.IndexSetName];

                    if (indexSetDirectory != null)
                    {
                        return new SearchingContext()
                        {
                            Analyzer = luceneSearcher.IndexingAnalyzer,
                            IndexSearcher = new IndexSearcher(indexSetDirectory, true), // TODO: handle reuse
                            EnableLeadingWildcards = luceneSearcher.EnableLeadingWildcards
                        };
                    }
                }
            }

            return null;
        }
    }
}
