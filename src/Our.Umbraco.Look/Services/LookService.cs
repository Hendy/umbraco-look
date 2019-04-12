using Examine.LuceneEngine;
using Lucene.Net.Spatial.Tier.Projectors;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using Umbraco.Web;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Flag to indicate whether the look service has been initialized
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// Locking obj to prevent multiple initialization
        /// </summary>
        private object _initializationLock = new object();

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
        /// Collection of indexer configuration models, keyed by indexer name.
        /// </summary>
        private Dictionary<string, IndexerConfiguration> _indexerConfigurations = new Dictionary<string, IndexerConfiguration>();

        /// <summary>
        /// Lucene directory representations for each of the Examine index sets
        /// </summary>
        private Dictionary<string, Directory> _indexSetDirectories;

        /// <summary>
        /// Function called before indexing
        /// </summary>
        private Action<IndexingContext> _beforeIndexing;

        /// <summary>
        /// Function to get the name for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, string> _nameIndexer;

        /// <summary>
        /// Function to get the date for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, DateTime?> _dateIndexer;

        /// <summary>
        /// Function to get text for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, string> _textIndexer;

        /// <summary>
        /// Function to get the tags for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, LookTag[]> _tagIndexer;

        /// <summary>
        /// Function to get a location for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, Location> _locationIndexer;

        /// <summary>
        /// Function called after indexing
        /// </summary>
        private Action<IndexingContext> _afterIndexing;

        /// <summary>
        /// Collection of cartesian tier plotters
        /// </summary>
        private List<CartesianTierPlotter> _cartesianTierPlotters = new List<CartesianTierPlotter>();

        /// <summary>
        /// Max distance in miles for distance searches and location indexing
        /// </summary>
        private static double _maxDistance => 10000; // 12450 = half circumfrence of earth TODO: make configuration

        /// <summary>
        /// max number of results to request for a lucene query
        /// </summary>
        private static int _maxLuceneResults => 5000; // TODO: make configurable (maybe part of the SearchQuery obj)

        /// <summary>
        /// Supplied by the initialization event (for re-use by the LookMatch)
        /// </summary>
        private UmbracoHelper _umbracoHelper;

        /// <summary>
        /// Enum used to indicate which Lucene fields should be returned
        /// </summary>
        private RequestFields _requestFields = RequestFields.AllFields;

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