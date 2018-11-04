using System;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing name
        /// </summary>
        /// <param name="nameFunc"></param>
        public static void SetNameIndexer(Func<IPublishedContent, string, string> nameFunc)
        {
            LogHelper.Info(typeof(LookService), "Name indexing function set");

            LookService.Instance.NameIndexer = nameFunc;
        }
    }
}
