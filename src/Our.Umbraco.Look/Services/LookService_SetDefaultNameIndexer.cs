using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Set a custom function to execute at index-time, which will return a string value for the name field (or null to not index)
        /// </summary>
        /// <param name="nameIndexer">The custom name indexing function</param>
        internal static void SetDefaultNameIndexer(Func<IndexingContext, string> nameIndexer)
        {
            LogHelper.Info(typeof(LookService), "Name indexing function set");

            LookService.Instance._defaultNameIndexer = nameIndexer;
        }
    }
}
