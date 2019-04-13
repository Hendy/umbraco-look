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
            var indexerConfiguration = LookService.GetIndexerConfiguration(indexingContext.IndexerName);

            if (!indexerConfiguration.ShouldIndexAlias(indexingContext.Item?.DocumentTypeAlias))
            {
                indexingContext.Cancel();
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                LookService.GetBeforeIndexing()(indexingContext);
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(LookService), "Error in global BeforeIndexing", exception);
            }

            try
            {
                LookService.GetBeforeIndexing(indexingContext.IndexerName)(indexingContext);
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(LookService), "Error in indexer BeforeIndexing", exception);
            }

            LookService.IndexNode(indexingContext, document);

            LookService.IndexName(indexingContext, document);

            LookService.IndexDate(indexingContext, document);

            LookService.IndexText(indexingContext, document);

            LookService.IndexTags(indexingContext, document);

            LookService.IndexLocation(indexingContext, document);

            try
            {
                LookService.GetAfterIndexing(indexingContext.IndexerName)(indexingContext);
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(LookService), "Error in indexer AfterIndexing", exception);
            }

            try
            {
                LookService.GetAfterIndexing()(indexingContext);
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(LookService), "Error in global AfterIndexing", exception);
            }

            stopwatch.Stop();

            if (!indexingContext.Cancelled)
            {
                LogHelper.Debug(typeof(LookService), $"Building Lucene Document for Id='{indexingContext.Item?.Id}', Key='{indexingContext.Item?.GetGuidKey()}', Name='{indexingContext.Item?.Name}' in Index '{ indexingContext.IndexerName }' Took { stopwatch.ElapsedMilliseconds }ms");
            }
        }
    }
}
