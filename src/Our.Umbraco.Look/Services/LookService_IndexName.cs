using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        private static void IndexName(IndexingContext indexingContext, Document document)
        {
            if (indexingContext.Cancelled) return;

            string name = null;

            if (LookService.GetNameIndexer() != null)
            {
                try
                {
                    name = LookService.GetNameIndexer()(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in name indexer", exception);
                }
            }
            else if (indexingContext.Item != null)
            {
                name = indexingContext.Item.Name;
            }

            if (name != null)
            {
                var hasNameField = new Field(
                        LookConstants.HasNameField,
                        "1",
                        Field.Store.NO,
                        Field.Index.NOT_ANALYZED);

                var nameField = new Field(
                                        LookConstants.NameField,
                                        name,
                                        Field.Store.YES,
                                        Field.Index.NOT_ANALYZED,
                                        Field.TermVector.YES);

                // field for lower case searching
                var nameFieldLowered = new Field(
                                        LookConstants.NameField + "_Lowered",
                                        name.ToLower(),
                                        Field.Store.NO,
                                        Field.Index.NOT_ANALYZED,
                                        Field.TermVector.YES);

                var nameSortedField = new Field(
                                            LuceneIndexer.SortedFieldNamePrefix + LookConstants.NameField,
                                            name.ToLower(), // force case insentive sorting
                                            Field.Store.NO,
                                            Field.Index.NOT_ANALYZED,
                                            Field.TermVector.NO);

                document.Add(hasNameField);
                document.Add(nameField);
                document.Add(nameFieldLowered);
                document.Add(nameSortedField);
            }
        }
    }
}
