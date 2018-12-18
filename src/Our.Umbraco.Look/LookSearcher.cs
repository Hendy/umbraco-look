using Examine;
using Examine.LuceneEngine.Providers;
using Examine.SearchCriteria;

namespace Our.Umbraco.Look
{
    public class LookSearcher : LuceneSearcher
    {
        public override ISearchResults Search(ISearchCriteria searchParams)
        {
            return this.Search(searchParams, int.MaxValue);
        }
        
        public override ISearchResults Search(ISearchCriteria searchParams, int maxResults)
        {
            // TODO: put NodeQuery, NameQuery, TextQuery, DateQuery, TagQuery & LocationQuery properties onto custom ISearchCriteria
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
        /// <param name="useWildcards">igored, as wildcards are always possible</param>
        /// <param name="indexType"></param>
        /// <returns></returns>
        public override ISearchResults Search(string searchText, bool useWildcards, string indexType)
        {
            return new LookQuery(this.Name) { TextQuery = new TextQuery(searchText) }.Run();
        }

        ////clean integration with Examine (consumer just has to cast Searcher to this type), but may conflict with LookQuery constructor (specifying a different searcher)
        //public ISearchResults Search(LookQuery lookQuery)
        //{
        //    lookQuery.SearcherName = this.Name;
        //    return lookQuery.Run();
        //}
    }
}
