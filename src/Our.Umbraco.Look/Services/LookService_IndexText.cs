using Lucene.Net.Documents;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        private static void IndexText(IndexingContext indexingContext, Document document)
        {
            if (indexingContext.Cancelled) return;

            var textIndexer = LookService.GetTextIndexer(indexingContext.IndexerName);

            if (textIndexer != null)
            {
                string text = null;

                try
                {
                    text = textIndexer(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in text indexer", exception);
                }

                if (text != null)
                {
                    var hasTextField = new Field(
                                            LookConstants.HasTextField,
                                            "1",
                                            Field.Store.NO,
                                            Field.Index.NOT_ANALYZED);

                    var textField = new Field(
                                            LookConstants.TextField,
                                            text,
                                            Field.Store.YES,
                                            Field.Index.ANALYZED,
                                            Field.TermVector.YES);

                    document.Add(hasTextField);
                    document.Add(textField);
                }
            }
        }
    }
}
