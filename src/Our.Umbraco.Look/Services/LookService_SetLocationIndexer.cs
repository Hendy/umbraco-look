using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing location
        /// </summary>
        /// <param name="locationIndexer">Your custom location indexing function</param>
        internal static void SetLocationIndexer(Func<IndexingContext, Location> locationIndexer)
        {
            LogHelper.Info(typeof(LookService), "Location indexing function set");

            LookService.Instance._locationIndexer = locationIndexer;
        }
    }
}
