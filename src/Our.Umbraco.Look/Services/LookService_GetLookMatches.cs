using Lucene.Net.Documents;
using Lucene.Net.Search;
using Our.Umbraco.Look.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Supplied with the result of a Lucene query, this method will yield a constructed LookMatch for each in order
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="indexSearcher">The searcher supplied to get the Lucene doc for each id in the Lucene results (topDocs)</param>
        /// <param name="topDocs">The results of the Lucene query (a collection of ids in an order)</param>
        /// <param name="requestFields">Enum value specifying which Lucene fields shoudl be returned</param>
        /// <param name="getHighlight">Function used to get the highlight text for a given result text</param>
        /// <param name="getDistance">Function used to calculate distance (if a location was supplied in the original query)</param>
        /// <returns></returns>
        internal static IEnumerable<LookMatch> GetLookMatches(
                                                    string searcherName,
                                                    IndexSearcher indexSearcher,
                                                    ScoreDoc[] scoreDocs,
                                                    RequestFields requestFields,
                                                    Func<string, IHtmlString> getHighlight,
                                                    Func<int, double?> getDistance)
        {
            MapFieldSelector mapFieldSelector = null; // when null, all fields are returned

            // these fields are always requested
            var lookFieldNames = new string[] {
                LookConstants.NodeIdField,
                LookConstants.NodeKeyField,
                LookConstants.NodeTypeField,
                LookConstants.NodeAliasField,
                LookConstants.HostIdField,
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

            var getHostId = new Func<string, int?>(x => {
                if (int.TryParse(x, out int id)) { return id; }
                return null;
            });

            var getItemGuid = new Func<string, Guid?>(x =>
            {
                if (Guid.TryParse(x, out Guid guid)) { return guid; }
                return null;
            });

            var getCultureInfo = new Func<string, CultureInfo>(x =>
            {
                if (int.TryParse(x, out int lcid)) { return new CultureInfo(lcid); }
                return null;
            });

            // there should always be a valid node type value to parse
            var getNodeType = new Func<string, ItemType>(x => 
            {
                Enum.TryParse(x, out ItemType type); return type;
            });
            
            // helper to simplify call below
            var getTags = new Func<Field[], LookTag[]>(x => {
                if (x != null) { return x.Select(y => new LookTag(y.StringValue())).ToArray(); }
                return new LookTag[] { };
            });

            foreach (var scoreDoc in scoreDocs)
            {
                var docId = scoreDoc.doc;

                var doc = indexSearcher.Doc(docId, mapFieldSelector);
                
                var lookMatch = new LookMatch(
                    searcherName,
                    scoreDoc.doc,
                    scoreDoc.score,
                    getHostId(doc.Get(LookConstants.HostIdField)), // could be null
                    Convert.ToInt32(doc.Get(LookConstants.NodeIdField)),
                    getItemGuid(doc.Get(LookConstants.NodeKeyField)), // this should only be null for unit tests (outside umbraco context)
                    doc.Get(LookConstants.NodeAliasField),
                    getCultureInfo(doc.Get(LookConstants.CultureField)),
                    doc.Get(LookConstants.NameField),
                    doc.Get(LookConstants.DateField).LuceneStringToDate(),
                    doc.Get(LookConstants.TextField),
                    getHighlight(doc.Get(LookConstants.TextField)),
                    getTags(doc.GetFields(LookConstants.AllTagsField)),
                    doc.Get(LookConstants.LocationField) != null ? Location.FromString(doc.Get(LookConstants.LocationField)) : null,
                    getDistance(docId),
                    getNodeType(doc.Get(LookConstants.NodeTypeField)),
                    LookService.Instance._umbracoHelper
                );

                // populate the Examine SearchResult.Fields collection
                if (requestFields == RequestFields.AllFields)
                {
                    string[] fieldNames = doc
                                            .GetFields()
                                            .Cast<Field>()
                                            .Select(x => x.Name())
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
                else
                {
                    // look fields only

                    // TODO: map fields                    
                }

                yield return lookMatch;
            }
        }
    }
}
