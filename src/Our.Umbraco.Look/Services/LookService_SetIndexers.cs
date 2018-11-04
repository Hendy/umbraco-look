using Our.Umbraco.Look.Models;
using System;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing text
        /// </summary>
        /// <param name="textFunc"></param>
        public static void SetTextIndexer(Func<IPublishedContent, string> textFunc)
        {
            LogHelper.Info(typeof(LookService), "Text indexing function set");

            LookService.Instance.TextIndexer = textFunc;
        }

        /// <summary>
        /// Register consumer code to perform when indexing tags
        /// </summary>
        /// <param name="tagsFunc"></param>
        public static void SetTagIndexer(Func<IPublishedContent, string[]> tagsFunc)
        {
            LogHelper.Info(typeof(LookService), "Tag indexing function set");

            LookService.Instance.TagIndexer = tagsFunc;
        }

        /// <summary>
        /// Register consumer code to perform when indexing date
        /// </summary>
        /// <param name="dateFunc"></param>
        public static void SetDateIndexer(Func<IPublishedContent, DateTime?> dateFunc)
        {
            LogHelper.Info(typeof(LookService), "Date indexing function set");

            LookService.Instance.DateIndexer = dateFunc;
        }

        /// <summary>
        /// Register consumer code to perform when indexing name
        /// </summary>
        /// <param name="nameFunc"></param>
        public static void SetNameIndexer(Func<IPublishedContent, string> nameFunc)
        {
            LogHelper.Info(typeof(LookService), "Name indexing function set");

            LookService.Instance.NameIndexer = nameFunc;
        }

        /// <summary>
        /// Register consumer code to perform when indexing location
        /// </summary>
        /// <param name="locationFunc"></param>
        public static void SetLocationIndexer(Func<IPublishedContent, Location> locationFunc)
        {
            LogHelper.Info(typeof(LookService), "Location indexing function set");

            LookService.Instance.LocationIndexer = locationFunc;
        }

    }
}
