using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Our.Umbraco.Look.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look
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

            // these fields are always requested
            var lookFieldNames = new string[] {
                LuceneIndexer.IndexNodeIdFieldName,  // "__NodeId"
                LookConstants.NodeTypeField,
                LookConstants.NameField,
                LookConstants.DateField,
                LookConstants.TextField,
                LookConstants.AllTagsField,
                LookConstants.LocationField
            };

            if (requestFields == RequestFields.LookFieldsOnly) 
            {
                // limit fields to be returned
                mapFieldSelector = new MapFieldSelector(lookFieldNames);
            }

            // there should always be a valid node type value to parse
            var getNodeType = new Func<string, PublishedItemType>(x => { Enum.TryParse(x, out PublishedItemType type); return type; });
            
            // helper to simplify call below
            var getTags = new Func<Field[], LookTag[]>(x => {
                if (x != null) { return x.Select(y => new LookTag(y.StringValue())).ToArray(); }
                return new LookTag[] { };
            });

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var docId = scoreDoc.doc;

                var doc = indexSearcher.Doc(docId, mapFieldSelector);

                var lookMatch = new LookMatch(
                    scoreDoc.doc,
                    scoreDoc.score,
                    Convert.ToInt32(doc.Get(LuceneIndexer.IndexNodeIdFieldName)),
                    doc.Get(LookConstants.NameField),
                    doc.Get(LookConstants.DateField).LuceneStringToDate(),
                    doc.Get(LookConstants.TextField),
                    getHighlight(doc.Get(LookConstants.TextField)),
                    getTags(doc.GetFields(LookConstants.AllTagsField)),
                    doc.Get(LookConstants.LocationField) != null ? Location.FromString(doc.Get(LookConstants.LocationField)) : null,
                    getDistance(docId),
                    getNodeType(doc.Get(LookConstants.NodeTypeField)),
                    LookService.Instance.UmbracoHelper
                );

                // populate the Examine SearchResult.Fields collection
                if (requestFields == RequestFields.AllFields)
                {
                    string[] fieldNames = doc
                                            .GetFields()
                                            .Cast<Field>()
                                            .Select(x => x.Name())
                                            .Where(x => !lookFieldNames.Contains(x)) // exclude Look fields
                                            .ToArray();

                    foreach (var fieldName in fieldNames)
                    {
                        var values = doc.GetValues(fieldName);

                        // replicating logic from Examine
                        if (values.Length > 1)
                        {
                            // TODO: reflection to set internal MultiValueFields

                            lookMatch.Fields[fieldName] = values[0];
                        }
                        else if (values.Length > 0)
                        {
                            lookMatch.Fields[fieldName] = values[0];
                        }
                    }
                }

                yield return lookMatch;
            }
        }
    }
}
