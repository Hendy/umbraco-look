using Lucene.Net.Spatial.Tier.Projectors;
using Lucene.Net.Store;
using Our.Umbraco.Look.Models;
using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    /// <summary>
    /// Useful references:
    /// https://gist.github.com/ismailmayat/3902c660527c8b3d20b38ae724ab9892
    /// http://www.d2digital.co.uk/blog/2015/08/lucenenet-indexer-geospatial-searching-and-umbraco
    /// </summary>
    public partial class LookService
    {
        /// <summary>
        /// Lucene directory representations for each of the Examine index sets
        /// </summary>
        private Dictionary<string, Directory> IndexSetDirectories { get; set; } = null;

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
        private Func<IndexingContext, string> TextIndexer { get; set; } = null;

        /// <summary>
        /// Function to get the tags for the IPublishedContent being indexed
        /// </summary>
        private Func<IndexingContext, string[]> TagIndexer { get; set; } = null;

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
        /// max numnber of results to request for a lucene query
        /// </summary>
        private static int MaxLuceneResults => 5000; // TODO: make configurable (maybe part of the SearchQuery obj)

        /// <summary>
        /// Access the singleton instance of this search service
        /// </summary>
        internal static LookService Instance => _lazy.Value;

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