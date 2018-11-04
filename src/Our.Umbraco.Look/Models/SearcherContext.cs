using Examine;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using System.Linq;
using UmbracoExamine;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Model specifying details as to how the search should interact with the Lucene index
    /// </summary>
    internal class SearcherContext
    {
        //internal string SearcherName { get; set; }

        internal string IndexSetName { get; set; }

        //internal System.IO.DirectoryInfo LuceneIndexFolder { get; set; }

        internal Analyzer Analyzer { get; set; }

        internal bool EnableLeadingWildcards { get; set; }

        internal IndexSearcher GetIndexSearcher()
        {
            // TODO: this method will handle the resuse of a returned IndexSearcher

            var indexProvider = ExamineManager // WARNING: this is slow
                                    .Instance
                                    .IndexProviderCollection
                                    .Select(x => x as BaseUmbracoIndexer)
                                    .Where(x => x != null)
                                    .Select(x => (BaseUmbracoIndexer)x)
                                    .FirstOrDefault(x => x.IndexSetName == this.IndexSetName);

            if (indexProvider != null)
            {
                return new IndexSearcher(indexProvider.GetLuceneDirectory(), true);
            }

            return null;
        }
    }
}
