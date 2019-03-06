using Lucene.Net.Spatial.Tier.Projectors;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using Umbraco.Web;
using Examine.LuceneEngine;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Flag used to indicate if the examine indexers to hook into have been configured
        /// (by default, all examine indexes will be hooked into unless the consumer says otherwise)
        /// </summary>
        private bool _examineIndexersConfigured = false;

        /// <summary>
        /// Collection of document indexing event handlers for Examine Umbraco indexes (collection persisted so that events can be de-registered)
        /// key = indexer name
        /// value = bound event
        /// </summary>
        private Dictionary<string, EventHandler<DocumentWritingEventArgs>> _examineDocumentWritingEvents = new Dictionary<string, EventHandler<DocumentWritingEventArgs>>();

        /// <summary>
        /// Flag to indicate whether the look service has been initialized
        /// </summary>
        private bool Initialized { get; set; } = false;

        /// <summary>
        /// Locking obj to prevent multiple initialization
        /// </summary>
        private object InitializationLock { get; set; } = new object();

        /// <summary>
        /// Collection of Examine indexer names that Look should hook into
        /// When null, indicates that all avaiable should be used
        /// </summary>
        [Obsolete]
        private string[] ExamineIndexers { get; set; } = null; // default to all

        /// <summary>
        /// Lucene directory representations for each of the Examine index sets
        /// </summary>
        private Dictionary<string, Directory> IndexSetDirectories { get; set; }

        /// <summary>
        /// Function to get the name for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, string> NameIndexer { get; set; } = x => x.Item.Name;

        /// <summary>
        /// Function to get the date for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, DateTime?> DateIndexer { get; set; } = x => x.Item.UpdateDate;

        /// <summary>
        /// Function to get text for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, string> TextIndexer { get; set; }

        /// <summary>
        /// Function to get the tags for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, LookTag[]> TagIndexer { get; set; }

        /// <summary>
        /// Function to get a location for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, Location> LocationIndexer { get; set; }

        /// <summary>
        /// Collection of cartesian tier plotters
        /// </summary>
        private List<CartesianTierPlotter> CartesianTierPlotters { get; } = new List<CartesianTierPlotter>();

        /// <summary>
        /// Max distance in miles for distance searches and location indexing
        /// </summary>
        private static double MaxDistance => 10000; // 12450 = half circumfrence of earth TODO: make configuration

        /// <summary>
        /// max number of results to request for a lucene query
        /// </summary>
        private static int MaxLuceneResults => 5000; // TODO: make configurable (maybe part of the SearchQuery obj)

        /// <summary>
        /// Supplied by the initialization event (for re-use by the LookMatch)
        /// </summary>
        private UmbracoHelper UmbracoHelper { get; set; }

        /// <summary>
        /// Enum used to indicate which Lucene fields should be returned
        /// </summary>
        private RequestFields RequestFields { get; set; } = RequestFields.AllFields;

        /// <summary>
        /// Access the singleton instance of this search service
        /// </summary>
        private static LookService Instance => _lazy.Value;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static readonly Lazy<LookService> _lazy = new Lazy<LookService>(() => new LookService());

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private LookService()
        {
        }
    }
}