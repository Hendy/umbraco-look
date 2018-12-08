
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing tags
        /// </summary>
        /// <param name="tagIndexer">Your custom tag indexing function</param>
        public static void SetTagIndexer(Func<IndexingContext, LookTag[]> tagIndexer)
        {
            if (LookService.Instance.TagIndexer == null)
            {
                LogHelper.Info(typeof(LookService), "Tag indexing function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Tag indexing function replaced");
            }
            
            LookService.Instance.TagIndexer = tagIndexer;
        }

    }
}
