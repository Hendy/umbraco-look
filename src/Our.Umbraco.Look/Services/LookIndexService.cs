using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using Our.Umbraco.Look.Models;
using System;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Our.Umbraco.Look.Extensions;

namespace Our.Umbraco.Look.Services
{
    public static class LookIndexService
    {
        /// <summary>
        /// Register consumer code to perform when indexing text
        /// </summary>
        /// <param name="textFunc"></param>
        public static void SetTextIndexer(Func<IPublishedContent, string> textFunc)
        {
            LogHelper.Info(typeof(LookService), "Text indexing function set");

            LookService.Instance.TextIndexer = textFunc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedContent"></param>
        /// <returns></returns>
        public static string DefaultTextIndexer(IPublishedContent publishedContent)
        {
            //TODO: extract text from all known text fields
            // extract and rip html fields

            return null;
        }

        /// <summary>
        /// Register consumer code to perform when indexing tags
        /// </summary>
        /// <param name="tagsFunc"></param>
        public static void SetTagIndexer(Func<IPublishedContent, string[]> tagsFunc)
        {
            LogHelper.Info(typeof(LookService), "Tag indexing function set");

            LookService.Instance.TagIndexer = tagsFunc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publishedContent"></param>
        /// <returns></returns>
        public static string[] DefaultTagIndexer(IPublishedContent publishedContent)
        {
            // TODO: look for known tag datatypes and pickers ?

            return null;
        }

        /// <summary>
        /// Register consumer code to perform when indexing date
        /// </summary>
        /// <param name="dateFunc"></param>
        public static void SetDateIndexer(Func<IPublishedContent, DateTime?> dateFunc)
        {
            LogHelper.Info(typeof(LookService), "Date indexing function set");

            LookService.Instance.DateIndexer = dateFunc;
        }

        /// <summary>
        /// By defult, returns the UpdateDate of the IPublishedContent
        /// </summary>
        /// <param name="publishedContent"></param>
        /// <returns></returns>
        public static DateTime? DefaultDateIndexer(IPublishedContent publishedContent)
        {
            return publishedContent.UpdateDate;
        }

        /// <summary>
        /// Register consumer code to perform when indexing name
        /// </summary>
        /// <param name="nameFunc"></param>
        public static void SetNameIndexer(Func<IPublishedContent, string> nameFunc)
        {
            LogHelper.Info(typeof(LookService), "Name indexing function set");

            LookService.Instance.NameIndexer = nameFunc;
        }

        /// <summary>
        /// By default, returns the Name of the IPublishedContent
        /// </summary>
        /// <param name="publishedContent"></param>
        /// <returns></returns>
        public static string DefaultNameIndexer(IPublishedContent publishedContent)
        {
            return publishedContent.Name;
        }

        /// <summary>
        /// Register consumer code to perform when indexing location
        /// </summary>
        /// <param name="locationFunc"></param>
        public static void SetLocationIndexer(Func<IPublishedContent, Location> locationFunc)
        {
            LogHelper.Info(typeof(LookService), "Location indexing function set");

            LookService.Instance.LocationIndexer = locationFunc;
        }

        /// <summary>
        /// Do the indexing and set the field values onto the Lucene document
        /// </summary>
        /// <param name="publishedContent">The IPublishedContent being indexed</param>
        /// <param name="document">The Lucene Document</param>
        internal static void Index(IPublishedContent publishedContent, Document document)
        {
            if (LookService.Instance.TextIndexer != null)
            {
                string text = null;

                try
                {
                    text = LookService.Instance.TextIndexer(publishedContent);
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
                string[] tags = null;

                try
                {
                    tags = LookService.Instance.TagIndexer(publishedContent);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in tag indexer", exception);
                }

                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        if (tag.IsValidTag())
                        {
                            var tagField = new Field(
                                                LookConstants.TagsField,
                                                tag,
                                                Field.Store.YES,
                                                Field.Index.NOT_ANALYZED);

                            document.Add(tagField);
                        }
                        else
                        {
                            LogHelper.Info(typeof(LookIndexService), $"Attemped to index an invalid tag string '{tag}'");
                        }
                    }
                }
            }

            if (LookService.Instance.DateIndexer != null)
            {
                DateTime? date = null;

                try
                {
                    date = LookService.Instance.DateIndexer(publishedContent);
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

            if (LookService.Instance.NameIndexer != null)
            {
                string name = null;

                try
                {
                    name = LookService.Instance.NameIndexer(publishedContent);
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

                    var nameSortedField = new Field(
                                                LuceneIndexer.SortedFieldNamePrefix + LookConstants.NameField,
                                                name.ToLower(), // force case insentive sorting
                                                Field.Store.NO,
                                                Field.Index.NOT_ANALYZED,
                                                Field.TermVector.NO);

                    document.Add(nameField);
                    document.Add(nameSortedField);
                }
            }

            if (LookService.Instance.LocationIndexer != null)
            {
                Location location = null;

                try
                {
                    location = LookService.Instance.LocationIndexer(publishedContent);
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

                    document.Add(locationField);

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
