using Examine.LuceneEngine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using Our.Umbraco.Look.Models;
using System;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void Index(IPublishedContent publishedContent, DocumentWritingEventArgs e)
        {
            if (LookService.Instance.TextIndexer != null)
            {
                try
                {
                    var text = LookService.Instance.TextIndexer(publishedContent);

                    if (text != null)
                    {
                        var textField = new Field(
                                                LookService.TextField, 
                                                text, 
                                                Field.Store.YES, 
                                                Field.Index.ANALYZED, 
                                                Field.TermVector.YES);

                        e.Document.Add(textField);
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in text indexer", exception);
                }
            }

            if (LookService.Instance.TagIndexer != null)
            {
                try
                {
                    var tags = LookService.Instance.TagIndexer(publishedContent);

                    if (tags != null)
                    {
                        foreach (var tag in tags.Where(x => !string.IsNullOrWhiteSpace(x)))
                        {
                            var tagField = new Field(
                                                LookService.TagsField, 
                                                tag, 
                                                Field.Store.YES, 
                                                Field.Index.NOT_ANALYZED);

                            e.Document.Add(tagField);
                        }

                    }
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in tag indexer", exception);
                }
            }

            if (LookService.Instance.DateIndexer != null)
            {
                try
                {
                    var date = LookService.Instance.DateIndexer(publishedContent);

                    if (date.HasValue)
                    {
                        // TODO: change to storing date type
                        var ticks = date.Value.Ticks;

                        var dateField = new NumericField(
                                                   LookService.DateField,
                                                   Field.Store.YES,
                                                   false)
                                               .SetLongValue(ticks);

                        var dateSortedField = new NumericField(
                                                        LuceneIndexer.SortedFieldNamePrefix + LookService.DateField,
                                                        Field.Store.NO, //we don't want to store the field because we're only using it to sort, not return data
                                                        true)
                                                    .SetLongValue(ticks);

                        e.Document.Add(dateField);
                        e.Document.Add(dateSortedField);
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in date indexer", exception);
                }
            }

            if (LookService.Instance.NameIndexer != null)
            {
                try
                {
                    var name = LookService.Instance.NameIndexer(publishedContent);

                    if (name != null)
                    {
                        var nameField = new Field(
                                                LookService.NameField,
                                                name,
                                                Field.Store.YES,
                                                Field.Index.NOT_ANALYZED,
                                                Field.TermVector.YES);

                        var nameSortedField = new Field(
                                                    LuceneIndexer.SortedFieldNamePrefix + LookService.NameField,
                                                    name.ToLower(),
                                                    Field.Store.NO,
                                                    Field.Index.NOT_ANALYZED,
                                                    Field.TermVector.NO);

                        e.Document.Add(nameField);
                        e.Document.Add(nameSortedField);
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in name indexer", exception);
                }
            }

            if (LookService.Instance.LocationIndexer != null)
            {
                try
                {
                    var location = LookService.Instance.LocationIndexer(publishedContent);

                    if (location != null)
                    {
                        var locationLatitudeField = new Field(
                                               LookService.LocationField + "_Latitude",
                                               NumericUtils.DoubleToPrefixCoded(location.Latitude),
                                               Field.Store.YES,
                                               Field.Index.NOT_ANALYZED);

                        var locationLongitudeField = new Field(
                                            LookService.LocationField + "_Longitude",
                                            NumericUtils.DoubleToPrefixCoded(location.Longitude),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED);


                        e.Document.Add(locationLatitudeField);
                        e.Document.Add(locationLongitudeField);

                        foreach (var cartesianTierPlotter in LookService.Instance.CartesianTierPlotters)
                        {
                            var boxId = cartesianTierPlotter.GetTierBoxId(location.Latitude, location.Longitude);

                            var tierField = new Field(
                                                cartesianTierPlotter.GetTierFieldName(),
                                                NumericUtils.DoubleToPrefixCoded(boxId),
                                                Field.Store.YES,
                                                Field.Index.NOT_ANALYZED_NO_NORMS);

                            e.Document.Add(tierField);
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in location indexer", exception);
                }
            }
        }
    }
}
