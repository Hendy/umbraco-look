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
        public static void SetTextIndexer(Func<IPublishedContent, string, string> textFunc)
        {
            LogHelper.Info(typeof(LookService), "Text indexing function set");

            LookService.Instance.TextIndexer = textFunc;
        }
    }
}
