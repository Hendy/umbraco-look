using Examine;
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
    public partial class LookService
    {
        /// <summary>
        /// Function to get text for the IPublishedContent being indexed
        /// </summary>
        private Func<IPublishedContent, string> TextIndexer { get; set; } = x => Indexing.DefaultTextIndexer(x);

        /// <summary>
        /// Function to get the tags for the IPublishedContent being indexed
        /// </summary>
        private Func<IPublishedContent, string[]> TagIndexer { get; set; } = x => Indexing.DefaultTagIndexer(x);

        /// <summary>
        /// Function to get the date for the IPublishedContent being indexed
        /// </summary>
        private Func<IPublishedContent, DateTime?> DateIndexer { get; set; } = x => Indexing.DefaultDateIndexer(x);

        /// <summary>
        /// Function to get the name for the IPublishedContent being indexed
        /// </summary>
        private Func<IPublishedContent, string> NameIndexer { get; set; } = x => Indexing.DefaultNameIndexer(x);

        /// <summary>
        /// Function to get a location for the IPublishedContent being indexed
        /// </summary>
        private Func<IPublishedContent, Location> LocationIndexer { get; set; } = x => null;

        /// <summary>
        /// Main indexing method
        /// </summary>
        /// <param name="publishedContent">The IPublishedContet being indexed</param>
        /// <param name="e"></param>
        internal static void Index(IPublishedContent publishedContent, IndexingNodeDataEventArgs e)
        {
            if (LookService.Instance.TextIndexer != null)
            {
                try
                {
                    var text = LookService.Instance.TextIndexer(publishedContent);

                    if (text != null)
                    {
                        e.Fields[LookService.TextField] = text;
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
                        e.Fields[LookService.TagsField] = string.Join(" ", tags.Where(x => !string.IsNullOrWhiteSpace(x)));
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
                        e.Fields[LookService.DateField] = date.Value.Ticks.ToString();
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
                        e.Fields[LookService.NameField] = name;
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
                        e.Fields[LookService.LocationField] = location.ToString();
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in location indexer", exception);
                }
            }
        }

        /// <summary>
        /// Used to create the additional search fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DocumentWriting(object sender, DocumentWritingEventArgs e)
        {
            if (e.Fields.ContainsKey(LookService.DateField)) // it's storing a date value as a long type
            {
                if (long.TryParse(e.Fields[LookService.DateField], out long ticks))
                {
                    e.Document.RemoveFields(LookService.DateField);

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

            if (e.Fields.ContainsKey(LookService.NameField))
            {
                var name = e.Fields[LookService.NameField];

                e.Document.RemoveFields(LookService.NameField);

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

            if (e.Fields.ContainsKey(LookService.LocationField))
            {
                var location = new Location(e.Fields[LookService.LocationField]);

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

        /// <summary>
        /// Used to namespace the index setters
        /// </summary>
        public static class Indexing
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

            public static string[] DefaultTagIndexer(IPublishedContent publishedContent)
            {
                // TODO: look for known tag datatypes and pickers
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

            public static DateTime? DefaultDateIndexer(IPublishedContent publishedContent)
            {
                return publishedContent.UpdateDate;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="nameFunc"></param>
            public static void SetNameIndexer(Func<IPublishedContent, string> nameFunc)
            {
                LogHelper.Info(typeof(LookService), "Name indexing function set");

                LookService.Instance.NameIndexer = nameFunc;
            }

            public static string DefaultNameIndexer(IPublishedContent publishedContent)
            {
                return publishedContent.Name;
            }

            public static void SetLocationIndexer(Func<IPublishedContent, Location> locationFunc)
            {
                LogHelper.Info(typeof(LookService), "Location indexing function set");

                LookService.Instance.LocationIndexer = locationFunc;
            }
        }
    }
}
