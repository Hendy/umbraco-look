using System;
using System.Linq;

namespace Our.Umbraco.Look
{
    public static class LookConfiguration
    {
        /// <summary>
        /// Hook indexing
        /// Set the index names of the exmaine indexes to hook into (if not set, it'll default to all)
        /// </summary>
        public static string[] ExamineIndexers
        {
            set
            {
                LookService.SetExamineIndexers(value);
            }
            get
            {
                return LookService.GetExamineIndexers().Select(x => x.Name).ToArray();
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
        /// Specify which fields to return in the result set
        /// </summary>
        public static RequestFields RequestFields { set { LookService.SetRequestFields(value); } }
    }
}
