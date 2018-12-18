using Examine;
using Examine.LuceneEngine.Providers;
using Examine.SearchCriteria;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Collections.Specialized;

namespace Our.Umbraco.Look
{
    public class LookDetachedSearcher : LuceneSearcher
    {
        //// did this get called ?
        //public override void Initialize(string name, NameValueCollection config)
        //{
        //    base.Initialize(name, config);
        //}

        //// not required to override, as it returns all lucene fields (TODO: test with tag generated fields)
        //protected override string[] GetSearchFields()
        //{
        //    var debug = base.GetSearchFields();
        //    return debug;
        //}

        //public override ISearchCriteria CreateSearchCriteria()
        //{
        //    var debug = base.CreateSearchCriteria();
        //    return debug;
        //}
        //public override ISearchCriteria CreateSearchCriteria(BooleanOperation defaultOperation)
        //{
        //    var debug = base.CreateSearchCriteria(defaultOperation);
        //    return debug;
        //}
        //public override ISearchCriteria CreateSearchCriteria(string type)
        //{
        //    var debug = base.CreateSearchCriteria(type);
        //    return debug;
        //}
        //public override ISearchCriteria CreateSearchCriteria(string type, BooleanOperation defaultOperation)
        //{
        //    var debug = base.CreateSearchCriteria(type, defaultOperation);
        //    return debug;
        //}

        //public override Searcher GetSearcher()
        //{
        //    var debug = base.GetSearcher();
        //    return debug;
        //}
        //protected override Directory GetLuceneDirectory()
        //{
        //    var debug = base.GetLuceneDirectory();
        //    return debug;
        //}
        //protected override IndexReader OpenNewReader()
        //{
        //    var debug = base.OpenNewReader();
        //    return debug;
        //}


        public override ISearchResults Search(ISearchCriteria searchParams)
        {
            var debug = new LookQuery(this.Name) { ExamineQuery = searchParams }.Run();
            return debug;
        }
        public override ISearchResults Search(ISearchCriteria searchParams, int maxResults)
        {
            var debug = new LookQuery(this.Name) { ExamineQuery = searchParams }.Run();
            return debug;
        }
        public override ISearchResults Search(string searchText, bool useWildcards)
        {
            var debug = new LookQuery(this.Name) { TextQuery = new TextQuery(searchText) }.Run();
            return debug;
        }
        public override ISearchResults Search(string searchText, bool useWildcards, string indexType)
        {
            var debug = new LookQuery(this.Name) { TextQuery = new TextQuery(searchText) }.Run();
            return debug;
        }


    }
}
