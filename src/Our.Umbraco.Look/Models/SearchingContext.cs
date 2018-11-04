using Lucene.Net.Analysis;
using Lucene.Net.Search;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Model specifying details as to how the search should interact with the Lucene index
    /// </summary>
    internal class SearchingContext
    {
        internal Analyzer Analyzer { get; set; }

        internal bool EnableLeadingWildcards { get; set; }

        internal IndexSearcher IndexSearcher { get; set; }
    }
}
