using System.Collections.Specialized;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Examine.SearchCriteria;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Our.Umbraco.Look;

namespace Our.Umbraco.Look
{
    public class LookSearcher : LuceneSearcher
    {
        public override ISearchCriteria CreateSearchCriteria()
        {
            return new LookSearchCriteria(base.CreateSearchCriteria());
        }

        public override ISearchCriteria CreateSearchCriteria(BooleanOperation defaultOperation)
        {
            return new LookSearchCriteria(base.CreateSearchCriteria(defaultOperation));
        }

        public override ISearchCriteria CreateSearchCriteria(string type)
        {
            return new LookSearchCriteria(base.CreateSearchCriteria(type));
        }

        public override ISearchCriteria CreateSearchCriteria(string type, BooleanOperation defaultOperation)
        {
            return new LookSearchCriteria(base.CreateSearchCriteria(type, defaultOperation));
        }

        //protected override Directory GetLuceneDirectory()
        //{
        //    return base.GetLuceneDirectory();
        //}

        //public override Searcher GetSearcher()
        //{
        //    return base.GetSearcher();
        //}


        protected override string[] GetSearchFields()
        {
            var debug = base.GetSearchFields();

            return debug;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            // the search defaults to Look behaviour which allows for leading wildcards
            this.EnableLeadingWildcards = true;
        }

        //protected override IndexReader OpenNewReader()
        //{
        //    return base.OpenNewReader();
        //}

        public override ISearchResults Search(ISearchCriteria searchParams)
        {
            return this.Search(searchParams, int.MaxValue);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public override ISearchResults Search(ISearchCriteria searchParams, int maxResults)
        {
            var lookQuery = new LookQuery(this.Name)
            {
                ExamineQuery = searchParams
            };

            // TODO: safety check, incase search criteria wasn't created by this searcher

            if (searchParams is LookSearchCriteria)
            {                
                // TODO: put NodeQuery, NameQuery, TextQuery, DateQuery, TagQuery & LocationQuery properties onto custom LookSearchCriteria
            }

            var lookResult = lookQuery.Run();

            return lookResult;
        }

        public override ISearchResults Search(string searchText, bool useWildcards)
        {
            return this.Search(searchText, useWildcards, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="useWildcards">ignored, as wildcards are always possible</param>
        /// <param name="indexType"></param>
        /// <returns></returns>
        public override ISearchResults Search(string searchText, bool useWildcards, string indexType)
        {
            var lookQuery = new LookQuery(this.Name)
            {
                TextQuery = new TextQuery(searchText)
            };

            var lookResult = lookQuery.Run();

            return lookResult;
        }

        ////clean integration with Examine (consumer just has to cast Searcher to this type), but may conflict with LookQuery constructor (specifying a different searcher)
        //public ISearchResults Search(LookQuery lookQuery)
        //{
        //    lookQuery.SearcherName = this.Name;
        //    return lookQuery.Run();
        //}
    }
}
