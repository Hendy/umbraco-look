using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register function to perform before indexing
        /// </summary>
        /// <param name="beforeIndexing">the function</param>
        internal static void SetBeforeIndexing(Action<IndexingContext> beforeIndexing)
        {
            LogHelper.Info(typeof(LookService), "BeforeIndexing function set");

            LookService.Instance._beforeIndexing = beforeIndexing;
        }
    }
}
