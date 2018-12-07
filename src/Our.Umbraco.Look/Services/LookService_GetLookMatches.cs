using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Our.Umbraco.Look.Extensions;
using Our.Umbraco.Look.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        /// <summary>
        /// Supplied with the result of a Lucene query, this method will yield a constructed LookMatch for each in order
        /// </summary>
        /// <param name="indexSearcher">The searcher supplied to get the Lucene doc for each id in the Lucene results (topDocs)</param>
        /// <param name="topDocs">The results of the Lucene query (a collection of ids in an order)</param>
        /// <param name="requestFields">Enum value specifying which Lucene fields shoudl be returned</param>
        /// <param name="getHighlight">Function used to get the highlight text for a given result text</param>
        /// <param name="getDistance">Function used to calculate distance (if a location was supplied in the original query)</param>
        /// <returns></returns>
        private static IEnumerable<LookMatch> GetLookMatches(
                                                    IndexSearcher indexSearcher,
                                                    TopDocs topDocs,
                                                    RequestFields requestFields,
                                                    Func<string, IHtmlString> getHighlight,
                                                    Func<int, double?> getDistance)
        {
            MapFieldSelector mapFieldSelector = null; // when null, all fields are returned
            var fieldNames = new string[] { };

            if (requestFields == RequestFields.LookFields) // limit fields to be returned
            {
                var lookFieldNames = new List<string>();

                lookFieldNames.Add(LuceneIndexer.IndexNodeIdFieldName); // "__NodeId"
                lookFieldNames.Add(LookConstants.NodeTypeField);
                lookFieldNames.Add(LookConstants.NameField);
                lookFieldNames.Add(LookConstants.DateField);
                lookFieldNames.Add(LookConstants.TextField);
                lookFieldNames.Add(LookConstants.AllTagsField); // single field used to store all tags (for quick re-construction)
                lookFieldNames.Add(LookConstants.LocationField);

                fieldNames = lookFieldNames.ToArray();

                mapFieldSelector = new MapFieldSelector(fieldNames);
            }

            // there should always be a valid node type value to parse
            var getNodeType = new Func<string, PublishedItemType>(x => { Enum.TryParse(x, out PublishedItemType type); return type; });
            
            // helper to simplify call below
            var getTags = new Func<Field[], LookTag[]>(x => {
                if (x != null) { return x.Select(y => new LookTag(y.StringValue())).ToArray(); }
                return new LookTag[] { };
            });

            Dictionary<string, string[]> fieldValues;

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var docId = scoreDoc.doc;

                var doc = indexSearcher.Doc(docId, mapFieldSelector);

                if (requestFields == RequestFields.AllFields)
                {
                    fieldNames = doc.GetFields().Cast<Field>().Select(x => x.Name()).ToArray();
                }

                fieldValues = new Dictionary<string, string[]>();

                foreach (var fieldName in fieldNames)
                {
                    fieldValues.Add(fieldName, doc.GetValues(fieldName));
                }

                var lookMatch = new LookMatch(
                    scoreDoc.doc,
                    scoreDoc.score,
                    fieldValues,
                    Convert.ToInt32(doc.Get(LuceneIndexer.IndexNodeIdFieldName)),
                    getNodeType(doc.Get(LookConstants.NodeTypeField)),
                    doc.Get(LookConstants.NameField),
                    doc.Get(LookConstants.DateField).LuceneStringToDate(),
                    doc.Get(LookConstants.TextField),
                    getHighlight(doc.Get(LookConstants.TextField)),
                    getTags(doc.GetFields(LookConstants.AllTagsField)),
                    doc.Get(LookConstants.LocationField) != null ? Location.FromString(doc.Get(LookConstants.LocationField)) : null,
                    getDistance(docId),                    
                    LookService.Instance.UmbracoHelper
                );
                
                yield return lookMatch;
            }
        }
    }
}
