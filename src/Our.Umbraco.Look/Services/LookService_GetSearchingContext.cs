using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Search;
using Our.Umbraco.Look.Models;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
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
            LuceneSearcher searcher = null;

            if (string.IsNullOrWhiteSpace(searcherName))
            {
                searcher = ExamineManager.Instance.DefaultSearchProvider as LuceneSearcher;
            }
            else
            {
                searcher = ExamineManager.Instance.SearchProviderCollection[searcherName] as LuceneSearcher;

                if (searcher == null && !searcherName.EndsWith("Searcher"))
                {
                    searcher = ExamineManager.Instance.SearchProviderCollection[searcherName + "Searcher"] as LuceneSearcher;
                }
            }

            if (searcher == null)
            {
                LogHelper.Debug(typeof(LookService), $"Unable to find Examine Lucene Searcher '{ searcherName }'");
            }
            else
            {
                var indexSetDirectory = LookService.Instance._indexSetDirectories[searcher.IndexSetName];

                if (indexSetDirectory != null)
                {
                    var indexSearcher = new IndexSearcher(indexSetDirectory, true); // TODO: handle reuse
                    indexSearcher.SetDefaultFieldSortScoring(true, true);

                    return new SearchingContext()
                    {
                        Analyzer = searcher.IndexingAnalyzer,
                        IndexSearcher = indexSearcher,
                        EnableLeadingWildcards = searcher.EnableLeadingWildcards
                    };
                }
            }

            return null;
        }
    }
}
