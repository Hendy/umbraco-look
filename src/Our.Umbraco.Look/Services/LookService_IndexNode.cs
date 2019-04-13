using Lucene.Net.Documents;
using Our.Umbraco.Look.Extensions;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        private static void IndexNode(IndexingContext indexingContext, Document document)
        {
            if (indexingContext.Cancelled) return;

            if (indexingContext.Item != null)
            {
                var publishedItemType = indexingContext?.HostItem?.ItemType ?? indexingContext.Item.ItemType;

                var hasNodeField = new Field(
                                            LookConstants.HasNodeField,
                                            "1",
                                            Field.Store.NO,
                                            Field.Index.NOT_ANALYZED);

                var nodeIdField = new Field(
                                            LookConstants.NodeIdField,
                                            indexingContext.Item.Id.ToString(),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED);

                var nodeKeyField = new Field(
                                            LookConstants.NodeKeyField,
                                            indexingContext.Item.GetGuidKey().GuidToLuceneString(),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED);

                var nodeTypeField = new Field(
                                            LookConstants.NodeTypeField,
                                            publishedItemType.ToString(),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED,
                                            Field.TermVector.NO);

                var nodeAliasField = new Field(
                                            LookConstants.NodeAliasField,
                                            indexingContext.Item.DocumentTypeAlias,
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED,
                                            Field.TermVector.NO);

                document.Add(hasNodeField);
                document.Add(nodeIdField);
                document.Add(nodeKeyField);
                document.Add(nodeTypeField);
                document.Add(nodeAliasField);

                if (publishedItemType == PublishedItemType.Content)
                {
                    var culture = indexingContext?.HostItem?.GetCulture() ?? indexingContext.Item.GetCulture();

                    if (culture != null)
                    {
                        var cultureField = new Field(
                                                LookConstants.CultureField,
                                                culture.LCID.ToString(),
                                                Field.Store.YES,
                                                Field.Index.NOT_ANALYZED,
                                                Field.TermVector.NO);

                        document.Add(cultureField);
                    }
                }

                if (indexingContext.HostItem != null)
                {
                    var isDetachedField = new Field(
                                                LookConstants.IsDetachedField,
                                                "1",
                                                Field.Store.NO,
                                                Field.Index.NOT_ANALYZED);

                    // indexing detached item, so store the host context id so we can return the detached item
                    var hostIdField = new Field(
                                            LookConstants.HostIdField,
                                            indexingContext.HostItem.Id.ToString(),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED);

                    document.Add(isDetachedField);
                    document.Add(hostIdField);
                }
            }
        }
    }
}
