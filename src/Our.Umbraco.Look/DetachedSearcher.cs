using Examine;
using Examine.LuceneEngine.Providers;
using Examine.SearchCriteria;

namespace Our.Umbraco.Look
{
    public class DetachedSearcher : LuceneSearcher
    {
        public override ISearchResults Search(ISearchCriteria searchParams)
        {
            return new LookQuery(this.Name) { ExamineQuery = searchParams }.Run();
        }
        
        public override ISearchResults Search(ISearchCriteria searchParams, int maxResults)
        {
            // TODO: pass max results into lookQuery
            return new LookQuery(this.Name) { ExamineQuery = searchParams }.Run();
        }

        public override ISearchResults Search(string searchText, bool useWildcards)
        {
            return this.Search(searchText, useWildcards, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="useWildcards">when true, the Text Field is searched, otherwise when false the Name Field is searched (to contain text)</param>
        /// <param name="indexType"></param>
        /// <returns></returns>
        public override ISearchResults Search(string searchText, bool useWildcards, string indexType)
        {
            var lookQuery = new LookQuery(this.Name);

            if (useWildcards)
            {
                lookQuery.TextQuery = new TextQuery(searchText);
            }
            else
            {
                lookQuery.NameQuery = new NameQuery(null, null, searchText);
            }

            return lookQuery.Run();
        }

        ////clean integration with Examine (consumer just has to cast Searcher to this type), but may conflict with LookQuery constructor (specifying a different searcher)
        //public ISearchResults Search(LookQuery lookQuery)
        //{
        //    lookQuery.SearcherName = this.Name;
        //    return lookQuery.Run();
        //}
    }
}
