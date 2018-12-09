using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing date
        /// </summary>
        /// <param name="dateIndexer">Your custom date indexing function</param>
        internal static void SetDateIndexer(Func<IndexingContext, DateTime?> dateIndexer)
        {
            if (LookService.Instance.DateIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Date indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Date indexing function replaced");
            }
            
            LookService.Instance.DateIndexer = dateIndexer;
        }
    }
}
