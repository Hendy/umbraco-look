using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        private static void IndexDate(IndexingContext indexingContext, Document document)
        {
            if (indexingContext.Cancelled) return;

            DateTime? date = null;

            var dateIndexer = LookService.GetDateIndexer(indexingContext.IndexerName);

            if (dateIndexer != null)
            {
                try
                {
                    date = dateIndexer(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in date indexer", exception);
                }
            }
            else if (indexingContext.Item != null)
            {
                date = indexingContext.HostItem?.UpdateDate ?? indexingContext.Item.UpdateDate;
            }

            if (date != null)
            {
                var hasDateField = new Field(
                                        LookConstants.HasDateField,
                                        "1",
                                        Field.Store.NO,
                                        Field.Index.NOT_ANALYZED);

                var dateValue = DateTools.DateToString(date.Value, DateTools.Resolution.SECOND);

                var dateField = new Field(
                                        LookConstants.DateField,
                                        dateValue,
                                        Field.Store.YES,
                                        Field.Index.ANALYZED,
                                        Field.TermVector.YES);

                var dateSortedField = new Field(
                                            LuceneIndexer.SortedFieldNamePrefix + LookConstants.DateField,
                                            dateValue,
                                            Field.Store.NO,
                                            Field.Index.NOT_ANALYZED,
                                            Field.TermVector.NO);

                document.Add(hasDateField);
                document.Add(dateField);
                document.Add(dateSortedField);
            }
        }
    }
}
