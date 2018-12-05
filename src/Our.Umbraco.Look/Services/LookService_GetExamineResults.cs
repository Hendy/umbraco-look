using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    public partial class LookService
    {
        private static IEnumerable<SearchResult> GetExamineResults(IndexSearcher indexSearcher, TopDocs topDocs)
        {
            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var docId = scoreDoc.doc;

                var doc = indexSearcher.Doc(docId);

                var searchResult = new SearchResult();

                searchResult.Id = Convert.ToInt32(doc.Get(LuceneIndexer.IndexNodeIdFieldName));
                searchResult.Score = scoreDoc.score;

                var fields = doc.GetFields();

                // coppied from Exmaine source
                foreach (var field in fields.Cast<Field>())
                {
                    var fieldName = field.Name();
                    var values = doc.GetValues(fieldName);

                    if (values.Length > 1)
                    {
                        // commented out & logging as internal method
                        //searchResult.MultiValueFields[fieldName] = values.ToList();
                        LogHelper.Debug(typeof(LookService), "Unable to support MultiValueFields with Examine - returning first value only");
                        
                        //ensure the first value is added to the normal fields
                        searchResult.Fields[fieldName] = values[0];
                    }
                    else if (values.Length > 0)
                    {
                        searchResult.Fields[fieldName] = values[0];
                    }
                }

                yield return searchResult;
            }
        }
    }
}
