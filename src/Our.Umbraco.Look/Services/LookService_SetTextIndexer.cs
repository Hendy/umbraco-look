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
        internal static void SetTextIndexer(Func<IndexingContext, string> textIndexer)
        {
            if (LookService.Instance._textIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Text indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Text indexing function replaced");
            }
            
            LookService.Instance._textIndexer = textIndexer;
        }
    }
}
