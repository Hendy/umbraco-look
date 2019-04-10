using Lucene.Net.Documents;
using Our.Umbraco.Look.Extensions;
using System;
using System.Diagnostics;
using Umbraco.Core.Logging;

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
                LogHelper.WarnWithException(typeof(LookService), "Error in BeforeIndexing", exception);
            }

            LookService.IndexNode(indexingContext, document);

            LookService.IndexName(indexingContext, document);

            LookService.IndexDate(indexingContext, document);

            LookService.IndexText(indexingContext, document);

            LookService.IndexTags(indexingContext, document);

            LookService.IndexLocation(indexingContext, document);

            try
            {
                LookService.GetAfterIndexing()(indexingContext);
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(LookService), "Error in AfterIndexing", exception);
            }

            stopwatch.Stop();

            if (!indexingContext.Cancelled)
            {
                LogHelper.Debug(typeof(LookService), $"Building Lucene Document For '{ indexingContext.Item.GetGuidKey() }' Took { stopwatch.ElapsedMilliseconds }ms");
            }
        }
    }
}
