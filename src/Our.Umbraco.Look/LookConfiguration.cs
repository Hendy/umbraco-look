using System;
using System.Linq;
using Our.Umbraco.Look.Services;

namespace Our.Umbraco.Look
{
    public static class LookConfiguration
    {
        /// <summary>
        /// 'Hook indexing'
        /// Get or set the index names of all the Exmaine indexes to hook into.
        /// If not set (or null) all Examine indexes are returned.
        /// </summary>
        public static string[] ExamineIndexers
        {
            get
            {
                return LookService
                        .GetExamineIndexers()
                        .Select(x => x.Name)
                        .ToArray();
            }
            set
            {
                LookService.SetExamineIndexers(value);
            }
        }

        /// <summary>
        /// Set a custom name indexer
        /// </summary>
        public static Func<IndexingContext, string> NameIndexer { set { LookService.SetNameIndexer(value); } }

        /// <summary>
        /// Set a custom date indexer
        /// </summary>
        public static Func<IndexingContext, DateTime?> DateIndexer { set { LookService.SetDateIndexer(value); } }

        /// <summary>
        /// Set a custom text indexer
        /// </summary>
        public static Func<IndexingContext, string> TextIndexer { set { LookService.SetTextIndexer(value); } }

        /// <summary>
        /// Set a custom tag indexer
        /// </summary>
        public static Func<IndexingContext, LookTag[]> TagIndexer { set { LookService.SetTagIndexer(value); } }

        /// <summary>
        /// Set a custom location indexer
        /// </summary>
        public static Func<IndexingContext, Location> LocationIndexer { set { LookService.SetLocationIndexer(value); } }

        /// <summary>
        /// Specify which fields to return in the result set.
        /// Defaults to AllFields which will return all Lucene fields for each document, making it fully populate the Fields dictionary property on the SearchResult.
        /// Setting it to LookFieldsOnly reduces the number of Lucene fields returned to the min required to inflate a LookMatch object.
        /// </summary>
        public static RequestFields RequestFields { set { LookService.SetRequestFields(value); } }
    }
}
