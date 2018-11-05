using Our.Umbraco.Look.Models;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing text
        /// </summary>
        /// <param name="textIndexer"></param>
        public static void SetTextIndexer(Func<IndexingContext, string> textIndexer)
        {
            LogHelper.Info(typeof(LookService), "Text indexing function set");

            LookService.Instance.TextIndexer = textIndexer;
        }
    }
}
