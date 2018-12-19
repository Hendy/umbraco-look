using System.Collections.Specialized;
using Examine;
using Examine.LuceneEngine.Providers;
using Examine.SearchCriteria;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Our.Umbraco.Look
{
    public class LookSearcher : LuceneSearcher
    {
        public override ISearchCriteria CreateSearchCriteria()
        {
            return base.CreateSearchCriteria();
        }

        public override ISearchCriteria CreateSearchCriteria(BooleanOperation defaultOperation)
        {
            return base.CreateSearchCriteria(defaultOperation);
        }

        public override ISearchCriteria CreateSearchCriteria(string type)
        {
            return base.CreateSearchCriteria(type);
        }

        public override ISearchCriteria CreateSearchCriteria(string type, BooleanOperation defaultOperation)
        {
            return base.CreateSearchCriteria(type, defaultOperation);
        }

        protected override Directory GetLuceneDirectory()
        {
            return base.GetLuceneDirectory();
        }

        public override Searcher GetSearcher()
        {
            return base.GetSearcher();
        }

        protected override string[] GetSearchFields()
        {
            return base.GetSearchFields();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        protected override IndexReader OpenNewReader()
        {
            return base.OpenNewReader();
        }

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
