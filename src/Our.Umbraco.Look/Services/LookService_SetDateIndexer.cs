using System;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing date
        /// </summary>
        /// <param name="dateFunc"></param>
        public static void SetDateIndexer(Func<IPublishedContent, string, DateTime?> dateFunc)
        {
            LogHelper.Info(typeof(LookService), "Date indexing function set");

            LookService.Instance.DateIndexer = dateFunc;
        }
    }
}
