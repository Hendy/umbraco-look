using Examine.LuceneEngine.Providers;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Our.Umbraco.Look.Exceptions;
using Our.Umbraco.Look.Models;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookQuery">The query to parse</param>
        /// <param name="parsingContext"></param>
        private static void ParseNameQuery(LookQuery lookQuery, ParsingContext parsingContext)
        {
            // handle sorting first, as name query clause not required for a name sort
            if (lookQuery.SortOn == SortOn.Name) // a -> z
            {
                parsingContext.Sort = new Sort(new SortField(LuceneIndexer.SortedFieldNamePrefix + LookConstants.NameField, SortField.STRING));
            }

            if (lookQuery.NameQuery != null)
            {
                parsingContext.QueryAdd(new TermQuery(new Term(LookConstants.HasNameField, "1")), BooleanClause.Occur.MUST);

                string wildcard1 = null;
                string wildcard2 = null; // incase Contains specified with StartsWith and/or EndsWith

                if (!string.IsNullOrEmpty(lookQuery.NameQuery.StartsWith))
                {
                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                    {
                        if (!lookQuery.NameQuery.Is.StartsWith(lookQuery.NameQuery.StartsWith))
                        {
                            throw new ParsingException("Conflict in NameQuery between Is and StartsWith");
                        }
                    }
                    else
                    {
                        wildcard1 = lookQuery.NameQuery.StartsWith + "*";
                    }
                }

                if (!string.IsNullOrEmpty(lookQuery.NameQuery.EndsWith))
                {
                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                    {
                        if (!lookQuery.NameQuery.Is.EndsWith(lookQuery.NameQuery.EndsWith))
                        {
                            throw new ParsingException("Conflict in NameQuery between Is and EndsWith");
                        }
                    }
                    else
                    {
                        if (wildcard1 == null)
                        {
                            wildcard1 = "*" + lookQuery.NameQuery.EndsWith;
                        }
                        else
                        {
                            wildcard1 += lookQuery.NameQuery.EndsWith;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(lookQuery.NameQuery.Contains))
                {
                    if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                    {
                        if (!lookQuery.NameQuery.Is.Contains(lookQuery.NameQuery.Contains))
                        {
                            throw new ParsingException("Conflict in NameQuery between Is and Contains");
                        }
                    }
                    else
                    {
                        if (wildcard1 == null)
                        {
                            wildcard1 = "*" + lookQuery.NameQuery.Contains + "*";
                        }
                        else
                        {
                            wildcard2 = "*" + lookQuery.NameQuery.Contains + "*";
                        }
                    }
                }

                var nameField = lookQuery.NameQuery.CaseSensitive ? LookConstants.NameField : LookConstants.NameField + "_Lowered";

                if (wildcard1 != null)
                {
                    var wildcard = lookQuery.NameQuery.CaseSensitive ? wildcard1 : wildcard1.ToLower();

                    parsingContext.QueryAdd(new WildcardQuery(new Term(nameField, wildcard)), BooleanClause.Occur.MUST);

                    if (wildcard2 != null)
                    {
                        wildcard = lookQuery.NameQuery.CaseSensitive ? wildcard2 : wildcard2.ToLower();

                        parsingContext.QueryAdd(new WildcardQuery(new Term(nameField, wildcard)), BooleanClause.Occur.MUST);
                    }
                }

                if (!string.IsNullOrEmpty(lookQuery.NameQuery.Is))
                {
                    var isText = lookQuery.NameQuery.CaseSensitive ? lookQuery.NameQuery.Is : lookQuery.NameQuery.Is.ToLower();

                    parsingContext.QueryAdd(new TermQuery(new Term(nameField, isText)), BooleanClause.Occur.MUST);
                }
            }
        }
    }
}
