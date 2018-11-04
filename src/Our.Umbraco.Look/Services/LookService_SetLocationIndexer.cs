using Our.Umbraco.Look.Models;
using System;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing location
        /// </summary>
        /// <param name="locationFunc"></param>
        public static void SetLocationIndexer(Func<IPublishedContent, string, Location> locationFunc)
        {
            LogHelper.Info(typeof(LookService), "Location indexing function set");

            LookService.Instance.LocationIndexer = locationFunc;
        }
    }
}
