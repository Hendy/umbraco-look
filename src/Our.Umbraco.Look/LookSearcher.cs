using Examine;
using Examine.LuceneEngine.Providers;
using Examine.SearchCriteria;
using System.Collections.Specialized;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Look Examine Searcher (extends Examine Lucene Searcher)
    /// </summary>
    public class LookSearcher : LuceneSearcher
    {
        /// <summary>
        /// Create new Look search criteria (which extends Examine search criteria)
        /// </summary>
        /// <returns></returns>
        public override ISearchCriteria CreateSearchCriteria()
        {
            return new LookSearchCriteria(base.CreateSearchCriteria());
        }

        /// <summary>
        /// Create new Look search criteria (which extends Examine search criteria)
        /// </summary>
        /// <param name="defaultOperation"></param>
        /// <returns></returns>
        public override ISearchCriteria CreateSearchCriteria(BooleanOperation defaultOperation)
        {
            return new LookSearchCriteria(base.CreateSearchCriteria(defaultOperation));
        }

        /// <summary>
        /// Create new Look search criteria (which extends Examine search criteria)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override ISearchCriteria CreateSearchCriteria(string type)
        {
            return new LookSearchCriteria(base.CreateSearchCriteria(type));
        }

        /// <summary>
        /// Create new Look search criteria (which extends Examine search criteria)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="defaultOperation"></param>
        /// <returns></returns>
        public override ISearchCriteria CreateSearchCriteria(string type, BooleanOperation defaultOperation)
        {
            return new LookSearchCriteria(base.CreateSearchCriteria(type, defaultOperation));
        }


        protected override string[] GetSearchFields()
        {
            var debug = base.GetSearchFields();

            return debug;
        }

        /// <summary>
        /// Initializes the Look Searcher provider
        /// </summary>
        /// <param name="name">The friendly name of this provider</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            this.EnableLeadingWildcards = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
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

            if (lookSearchCriteria != null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="useWildcards"></param>
        /// <returns></returns>
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
