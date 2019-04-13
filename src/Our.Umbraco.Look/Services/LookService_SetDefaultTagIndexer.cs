using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing tags
        /// </summary>
        /// <param name="tagIndexer">Your custom tag indexing function</param>
        internal static void SetDefaultTagIndexer(Func<IndexingContext, LookTag[]> tagIndexer)
        {
            LogHelper.Info(typeof(LookService), "Tag indexing function set");
            
            LookService.Instance._defaultTagIndexer = tagIndexer;
        }
    }
}
