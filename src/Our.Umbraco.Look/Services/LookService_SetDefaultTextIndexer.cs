using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing text
        /// </summary>
        /// <param name="textIndexer">Your custom text indexing function</param>
        internal static void SetDefaultTextIndexer(Func<IndexingContext, string> textIndexer)
        {
            LogHelper.Info(typeof(LookService), "Text indexing function set");
            
            LookService.Instance._defaultTextIndexer = textIndexer;
        }
    }
}
