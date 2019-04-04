using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform before indexing
        /// </summary>
        /// <param name="indexIf">Your custom function to determine if indexing should occur</param>
        internal static void IndexIf(Func<IndexingContext, bool> indexIf)
        {
            if (LookService.Instance._indexIf == null)
            {
                LogHelper.Info(typeof(LookService), "Index if function set");
            }
            else
            {
                LogHelper.Warn(typeof(LookService), "Index if function replaced");
            }

            LookService.Instance._indexIf = indexIf;
        }
    }
}
