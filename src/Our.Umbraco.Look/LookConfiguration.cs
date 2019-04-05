using Our.Umbraco.Look.Services;
using System;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Set properties on this static class to configure how Look operates
    /// </summary>
    public static class LookConfiguration
    {
        /// <summary>
        /// 'Hook indexing'
        /// Get or set the index names of all the Exmaine indexes to hook into.
        /// supplying a null or empty array means no examine indexers will be hooked into (by default if this isn't set, then all examine indexers will be hooked into)
        /// </summary>
        public static string[] ExamineIndexers { get { return LookService.GetExamineIndexers(); } set { LookService.SetExamineIndexers(value); } }

        /// <summary>
        /// Set a function that will be executed as the first indexing step to determine if the item should be indexed at all.
        /// This will only work fully in preventing a Lucene document from being created with a Look indexer. Examine indexers will still maintain their behaviour, but Look will not add any additional data into them.
        /// </summary>
        public static Action<IndexingContext> BeforeIndexing { set { LookService.SetBeforeIndexing(value); } }

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
        /// Flag to indicate whether a name indexer is enabled
        /// </summary>
        public static bool NameIndexerIsSet => LookService.GetNameIndexer() != null;

        /// <summary>
        /// Flag to indicate whether a date indexer is enabled
        /// </summary>
        public static bool DateIndexerIsSet => LookService.GetDateIndexer() != null;

        /// <summary>
        /// Flag to indicate whether a text indexer is enabled
        /// </summary>
        public static bool TextIndexerIsSet => LookService.GetTextIndexer() != null;

        /// <summary>
        /// Flag to indicate whether a tag indexer is enabled
        /// </summary>
        public static bool TagIndexerIsSet => LookService.GetTagIndexer() != null;

        /// <summary>
        /// Flag to indicate whether a location indexer is enabled
        /// </summary>
        public static bool LocationIndexerIsSet => LookService.GetLocationIndexer() != null;

        /// <summary>
        /// Specify which fields to return in the result set.
        /// Defaults to AllFields which will return all Lucene fields for each document, making it fully populate the Fields dictionary property on the SearchResult.
        /// Setting it to LookFieldsOnly reduces the number of Lucene fields returned to the min required to inflate a LookMatch object.
        /// </summary>
        public static RequestFields RequestFields { set { LookService.SetRequestFields(value); } }
    }
}
