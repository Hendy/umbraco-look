using Our.Umbraco.Look.Models;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing tags
        /// </summary>
        /// <param name="tagIndexer"></param>
        public static void SetTagIndexer(Func<IndexingContext, Tag[]> tagIndexer)
        {
            LogHelper.Info(typeof(LookService), "Tag indexing function set");

            LookService.Instance.TagIndexer = tagIndexer;
        }

    }
}
