using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using Our.Umbraco.Look.Models;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        ///  Do the indexing and set the field values onto the Lucene document
        /// </summary>
        /// <param name="indexingContext"></param>
        /// <param name="document"></param>
        internal static void Index(IndexingContext indexingContext, Document document)
        {
            if (indexingContext.Item != null)
            {
                var nodeTypeField = new Field(
                                            LookConstants.NodeTypeField,
                                            indexingContext.Item.ItemType.ToString(),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED,
                                            Field.TermVector.NO);

                document.Add(nodeTypeField);
            }

            if (LookService.Instance.NameIndexer != null)
            {
                string name = null;

                try
                {
                    name = LookService.Instance.NameIndexer(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in name indexer", exception);
                }

                if (name != null)
                {
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

                    document.Add(nameField);
                    document.Add(nameFieldLowered);
                    document.Add(nameSortedField);
                }
            }

            if (LookService.Instance.DateIndexer != null)
            {
                DateTime? date = null;

                try
                {
                    date = LookService.Instance.DateIndexer(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in date indexer", exception);
                }

                if (date != null)
                {
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

                    document.Add(dateField);
                    document.Add(dateSortedField);
                }
            }

            if (LookService.Instance.TextIndexer != null)
            {
                string text = null;

                try
                {
                    text = LookService.Instance.TextIndexer(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in text indexer", exception);
                }

                if (text != null)
                {
                    var textField = new Field(
                                            LookConstants.TextField,
                                            text,
                                            Field.Store.YES,
                                            Field.Index.ANALYZED,
                                            Field.TermVector.YES);

                    document.Add(textField);
                }
            }

            if (LookService.Instance.TagIndexer != null)
            {
                LookTag[] tags = null;

                try
                {
                    tags = LookService.Instance.TagIndexer(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in tag indexer", exception);
                }

                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        // add all tags to a common field (serialized such that Tag objects can be restored from this)
                        var allTagsField = new Field(
                                            LookConstants.AllTagsField,
                                            tag.ToString(),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED);

                        // add the tag value to a specific field - this is used for searching on
                        var tagField = new Field(
                                            LookConstants.TagsField + tag.Group,
                                            tag.Name,
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED);

                        document.Add(allTagsField);
                        document.Add(tagField);
                    }
                }
            }

            if (LookService.Instance.LocationIndexer != null)
            {
                Location location = null;

                try
                {
                    location = LookService.Instance.LocationIndexer(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in location indexer", exception);
                }

                if (location != null)
                {
                    var locationField = new Field(
                                                LookConstants.LocationField,
                                                location.ToString(),
                                                Field.Store.YES,
                                                Field.Index.NOT_ANALYZED);

                    var locationLatitudeField = new Field(
                                           LookConstants.LocationField + "_Latitude",
                                           NumericUtils.DoubleToPrefixCoded(location.Latitude),
                                           Field.Store.YES,
                                           Field.Index.NOT_ANALYZED);

                    var locationLongitudeField = new Field(
                                        LookConstants.LocationField + "_Longitude",
                                        NumericUtils.DoubleToPrefixCoded(location.Longitude),
                                        Field.Store.YES,
                                        Field.Index.NOT_ANALYZED);

                    document.Add(locationField);
                    document.Add(locationLatitudeField);
                    document.Add(locationLongitudeField);

                    foreach (var cartesianTierPlotter in LookService.Instance.CartesianTierPlotters)
                    {
                        var boxId = cartesianTierPlotter.GetTierBoxId(location.Latitude, location.Longitude);

                        var tierField = new Field(
                                            cartesianTierPlotter.GetTierFieldName(),
                                            NumericUtils.DoubleToPrefixCoded(boxId),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED_NO_NORMS);

                        document.Add(tierField);
                    }
                }
            }
        }
    }
}
