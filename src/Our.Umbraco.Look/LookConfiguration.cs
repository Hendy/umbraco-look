using Our.Umbraco.Look.Services;
using System;
using System.Collections.Generic;

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
        /// Set configuration behaviour on a per indexer basis.
        /// </summary>
        public static Dictionary<string, IndexerConfiguration> IndexerConfiguration => LookService.GetIndexerConfigurations();

        /// <summary>
        /// (Optional) custom method that can be called before the indexing of each IPublishedContent item.
        /// TODO: rename to DefaultBeforeIndexing ?
        /// </summary>
        public static Action<IndexingContext> BeforeIndexing { set { LookService.SetBeforeIndexing(value); } }

        /// <summary>
        /// (Optional) set a custom name indexer.
        /// By default, the IPublishedContent.Name value will be indexed.
        /// TODO: rename to DefaultNameIndexer ?
        /// </summary>
        public static Func<IndexingContext, string> NameIndexer { set { LookService.SetNameIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom date indexer.
        /// By default, the IPublishedContent.UpdateDate value will be indexed. (Detached items use their Host value)
        /// TODO: rename to DefaultDateIndexer ?
        /// </summary>
        public static Func<IndexingContext, DateTime?> DateIndexer { set { LookService.SetDateIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom text indexer.
        /// By default, no value is indexed.
        /// TODO: rename to DefaultTextIndexer ?
        /// </summary>
        public static Func<IndexingContext, string> TextIndexer { set { LookService.SetTextIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom tag indexer.
        /// By default, no value is indexed.
        /// TODO: rename to DefaultTagIndexer ?
        /// </summary>
        public static Func<IndexingContext, LookTag[]> TagIndexer { set { LookService.SetTagIndexer(value); } }

        /// <summary>
        /// (Optional) Set a custom location indexer.
        /// By default, no value is indexed.
        /// TODO: rename to DefaultLocationIndexer ?
        /// </summary>
        public static Func<IndexingContext, Location> LocationIndexer { set { LookService.SetLocationIndexer(value); } }

        /// <summary>
        /// (Optional) custom method that can be called after the indexing of each IPublishedContent item.
        /// TODO: rename to DefaultAfterIndexing ?
        /// </summary>
        public static Action<IndexingContext> AfterIndexing { set { LookService.SetAfterIndexing(value); } }

        /// <summary>
        /// Specify which fields to return in the result set.
        /// Defaults to AllFields which will return all Lucene fields for each document, making it fully populate the Fields dictionary property on the SearchResult.
        /// Setting it to LookFieldsOnly reduces the number of Lucene fields returned to the min required to inflate a LookMatch object.
        /// </summary>
        public static RequestFields RequestFields { set { LookService.SetRequestFields(value); } }
    }
}
