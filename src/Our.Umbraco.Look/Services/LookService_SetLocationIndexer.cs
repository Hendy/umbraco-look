using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing location
        /// </summary>
        /// <param name="locationIndexer">Your custom location indexing function</param>
        public static void SetLocationIndexer(Func<IndexingContext, Location> locationIndexer)
        {
            if (LookService.Instance.LocationIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Location indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Location indexing function replaced");
            }

            LookService.Instance.LocationIndexer = locationIndexer;
        }
    }
}
