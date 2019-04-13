using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register function to perform after indexing
        /// </summary>
        /// <param name="afterIndexing">the function</param>
        internal static void SetAfterIndexing(Action<IndexingContext> afterIndexing)
        {
            LogHelper.Info(typeof(LookService), "AfterIndexing function set");

            LookService.Instance._afterIndexing = afterIndexing;
        }
    }
}
