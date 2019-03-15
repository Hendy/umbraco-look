using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using Our.Umbraco.Look.Extensions;
using System;
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
            #region Node

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
                                            Field.Store.NO,
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

            #endregion

            #region Name

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

            #endregion

            #region Date

            DateTime? date = null;

            if (LookService.GetDateIndexer() != null)
            {
                try
                {
                    date = LookService.GetDateIndexer()(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in date indexer", exception);
                }
            }
            else if (indexingContext.Item != null)
            {
                date = indexingContext.Item.UpdateDate;
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

            #endregion

            #region Text

            if (LookService.GetTextIndexer() != null)
            {
                string text = null;

                try
                {
                    text = LookService.GetTextIndexer()(indexingContext);
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

            #endregion

            #region Tag

            if (LookService.GetTagIndexer() != null)
            {
                LookTag[] tags = null;

                try
                {
                    tags = LookService.GetTagIndexer()(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in tag indexer", exception);
                }

                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        var hasTagsField = new Field(
                                                LookConstants.HasTagsField,
                                                "1",
                                                Field.Store.NO,
                                                Field.Index.NOT_ANALYZED);

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

                        document.Add(hasTagsField);
                        document.Add(allTagsField);
                        document.Add(tagField);
                    }
                }
            }

            #endregion

            #region Location

            if (LookService.GetLocationIndexer() != null)
            {
                Location location = null;

                try
                {
                    location = LookService.GetLocationIndexer()(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in location indexer", exception);
                }

                if (location != null)
                {
                    var hasLocationField = new Field(
                                                LookConstants.HasLocationField,
                                                "1",
                                                Field.Store.NO,
                                                Field.Index.NOT_ANALYZED);

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

                    document.Add(hasLocationField);
                    document.Add(locationField);
                    document.Add(locationLatitudeField);
                    document.Add(locationLongitudeField);

                    foreach (var cartesianTierPlotter in LookService.Instance._cartesianTierPlotters)
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

            #endregion  
        }
    }
}
