using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Our.Umbraco.Look.Exceptions;
using Our.Umbraco.Look.Models;
using System.IO;
using System.Web;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookQuery">The query to parse</param>
        /// <param name="parsingContext"></param>
        private static void ParseTextQuery(LookQuery lookQuery, ParsingContext parsingContext)
        {
            if (lookQuery.TextQuery == null) return;
            
            parsingContext.QueryAdd(new TermQuery(new Term(LookConstants.HasTextField, "1")), BooleanClause.Occur.MUST);

            if (!string.IsNullOrWhiteSpace(lookQuery.TextQuery.SearchText))
            {
                var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, LookConstants.TextField, lookQuery.SearchingContext.Analyzer);

                Query searchTextQuery = null;

                try
                {
                    searchTextQuery = queryParser.Parse(lookQuery.TextQuery.SearchText);
                }
                catch
                {
                    throw new ParsingException($"Unable to parse LookQuery.TextQuery.SearchText: '{ lookQuery.TextQuery.SearchText }' into a Lucene query");
                }

                if (searchTextQuery != null)
                {
                    parsingContext.QueryAdd(searchTextQuery, BooleanClause.Occur.MUST);

                    if (lookQuery.TextQuery.GetHighlight)
                    {
                        var queryScorer = new QueryScorer(searchTextQuery.Rewrite(lookQuery.SearchingContext.IndexSearcher.GetIndexReader()));

                        var highlighter = new Highlighter(new SimpleHTMLFormatter("<strong>", "</strong>"), queryScorer);

                        parsingContext.GetHighlight = (x) =>
                        {
                            var tokenStream = lookQuery.SearchingContext.Analyzer.TokenStream(LookConstants.TextField, new StringReader(x));

                            var highlight = highlighter.GetBestFragments(
                                                            tokenStream,
                                                            x,
                                                            1, // max number of fragments
                                                            "...");

                            return new HtmlString(highlight);
                        };
                    }
                }
            }
        }
    }
}
