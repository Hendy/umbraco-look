using Lucene.Net.Spatial.Tier.Projectors;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using Umbraco.Web;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Lucene directory representations for each of the Examine index sets
        /// </summary>
        private Dictionary<string, Directory> IndexSetDirectories { get; set; } = null;

        /// <summary>
        /// Function to get the name for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, string> NameIndexer { get; set; } = null;

        /// <summary>
        /// Function to get the date for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, DateTime?> DateIndexer { get; set; } = null;

        /// <summary>
        /// Function to get text for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, string> TextIndexer { get; set; } = null;

        /// <summary>
        /// Function to get the tags for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, LookTag[]> TagIndexer { get; set; } = null;

        /// <summary>
        /// Function to get a location for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, Location> LocationIndexer { get; set; } = null;

        /// <summary>
        /// Collection of cartesian tier plotters
        /// </summary>
        private List<CartesianTierPlotter> CartesianTierPlotters { get; } = new List<CartesianTierPlotter>();

        /// <summary>
        /// Max distance in miles for distance searches & location indexing
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
        /// Singleton constructor (used privately to maintain state for consumer registered indexer functions)
        /// </summary>
        private LookService()
        {
        }
    }
}