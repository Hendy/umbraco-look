using Examine;
using Examine.LuceneEngine.Providers;
using Examine.SearchCriteria;
using System.Collections.Specialized;

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

        protected override string[] GetSearchFields()
        {
            var debug = base.GetSearchFields();

            return debug;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            this.EnableLeadingWildcards = false;
        }

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

            var lookSearchCriteria = searchParams as LookSearchCriteria;

            if (lookSearchCriteria != null) // TODO: safety check it was created by this searcher ?
            {
                lookQuery.ExamineQuery = lookSearchCriteria.ExamineQuery;
                lookQuery.NodeQuery = lookSearchCriteria.NodeQuery;
                lookQuery.NameQuery = lookSearchCriteria.NameQuery;
                lookQuery.DateQuery = lookSearchCriteria.DateQuery;
                lookQuery.TextQuery = lookSearchCriteria.TextQuery;
                lookQuery.TagQuery = lookSearchCriteria.TagQuery;
                lookQuery.LocationQuery = lookSearchCriteria.LocationQuery;
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
    }
}
