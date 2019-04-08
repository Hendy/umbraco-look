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
        /// Get or set the indexer names of all the Exmaine indexers to hook into.
        /// By default, all Umbraco Examine indexers are hooked into.
        /// Set to null (or an empty array) to remove all hooks. 
        /// </summary>
        public static string[] ExamineIndexers { get { return LookService.GetExamineIndexers(); } set { LookService.SetExamineIndexers(value); } }

        /// <summary>
        /// (Optional) custom method that can be called before the indexing of each IPublishedContent item.
        /// </summary>
        public static Action<IndexingContext> BeforeIndexing { set { LookService.SetBeforeIndexing(value); } }

        /// <summary>
        /// (Optional) set a custom name indexer.
        /// By default, the IPublishedContent.Name value will be indexed.
        /// </summary>
        public static Func<IndexingContext, string> NameIndexer { set { LookService.SetNameIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom date indexer.
        /// By default, the IPublishedContent.UpdateDate value will be indexed. (Detached items use their Host value)
        /// </summary>
        public static Func<IndexingContext, DateTime?> DateIndexer { set { LookService.SetDateIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom text indexer.
        /// By default, no value is indexed.
        /// </summary>
        public static Func<IndexingContext, string> TextIndexer { set { LookService.SetTextIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom tag indexer.
        /// By default, no value is indexed.
        /// </summary>
        public static Func<IndexingContext, LookTag[]> TagIndexer { set { LookService.SetTagIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom location indexer.
        /// By default, no value is indexed.
        /// </summary>
        public static Func<IndexingContext, Location> LocationIndexer { set { LookService.SetLocationIndexer(value); } }

        /// <summary>
        /// (Optional) custom method that can be called after the indexing of each IPublishedContent item.
        /// </summary>
        public static Action<IndexingContext> AfterIndexing { set { LookService.SetAfterIndexing(value); } }

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
