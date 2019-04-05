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
        internal static void BeforeIndexing(Func<IndexingContext, bool> beforeIndexing)
        {
            if (LookService.Instance._beforeIndexing == null)
            {
                LogHelper.Info(typeof(LookService), "BeforeIndexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "BeforeIndexing function replaced");
            }

            LookService.Instance._beforeIndexing = beforeIndexing;
        }
    }
}
