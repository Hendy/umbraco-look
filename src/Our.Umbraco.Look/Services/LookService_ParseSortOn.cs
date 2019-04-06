using Examine.LuceneEngine.Providers;
using Lucene.Net.Search;
using Our.Umbraco.Look.Models;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        private static void ParseSortOn(LookQuery lookQuery, ParsingContext parsingContext)
        {
            switch (lookQuery.SortOn)
            {
                case SortOn.Name: // a -> z
                    parsingContext.Sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.NameField, SortField.STRING));
                    break;

                case SortOn.DateAscending: // oldest -> newest
                    parsingContext.Sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField, SortField.LONG, false));
                    break;

                case SortOn.DateDescending: // newest -> oldest
                    parsingContext.Sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField, SortField.LONG, true));
                    break;

                // SortOn.Distance already set (if valid)
            }
        }
    }
}
