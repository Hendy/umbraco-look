using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform before indexing
        /// </summary>
        /// <param name="beforeIndexing">Your custom function to determine if indexing should occur</param>
        internal static void SetBeforeIndexing(Action<IndexingContext> beforeIndexing)
        {
            LogHelper.Info(typeof(LookService), "BeforeIndexing function set");

            LookService.Instance._beforeIndexing = beforeIndexing;
        }
    }
}
