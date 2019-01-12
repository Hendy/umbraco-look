using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look
{
    internal partial class LookService
    {
        /// <summary>
        /// Set a custom function to execute at index-time, which will return a string value for the name field (or null to not index)
        /// </summary>
        /// <param name="nameIndexer">The custom name indexing function</param>
        internal static void SetNameIndexer(Func<IndexingContext, string> nameIndexer)
        {
            if (LookService.Instance.NameIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Name indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Name indexing function replaced");
            }

            LookService.Instance.NameIndexer = nameIndexer;
        }
    }
}
