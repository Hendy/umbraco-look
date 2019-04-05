using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using Our.Umbraco.Look.Extensions;
using System;
using System.Diagnostics;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        ///  Do the indexing and set the field values onto the Lucene document
        /// </summary>
        /// <param name="indexingContext"></param>
        /// <param name="document"></param>
        internal static void Index(IndexingContext indexingContext, Document document)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                LookService.GetBeforeIndexing()(indexingContext);
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(LookService), "Error in before indexing", exception);
            }

            LookService.IndexNode(indexingContext, document);

            LookService.IndexName(indexingContext, document);

            LookService.IndexDate(indexingContext, document);

            LookService.IndexText(indexingContext, document);

            LookService.IndexTags(indexingContext, document);

            LookService.IndexLocation(indexingContext, document);

            stopwatch.Stop();
            LogHelper.Debug(typeof(LookService), $"Building Lucene Document For '{ indexingContext.Item.GetGuidKey() }' Took { stopwatch.ElapsedMilliseconds }ms");
        }
    }
}
