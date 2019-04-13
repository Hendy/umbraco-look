using Lucene.Net.Index;
using Lucene.Net.Search;
using Our.Umbraco.Look.Models;
using System.Linq;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookQuery">The query to parse</param>
        /// <param name="parsingContext"></param>
        private static void ParseTagQuery(ParsingContext parsingContext, LookQuery lookQuery)
        {
            if (lookQuery.TagQuery == null) return;

            parsingContext.QueryAdd(new TermQuery(new Term(LookConstants.HasTagsField, "1")), BooleanClause.Occur.MUST);

            // Has
            if (lookQuery.TagQuery.Has != null)
            {
                parsingContext.QueryAdd(
                        new TermQuery(new Term(LookConstants.TagsField + lookQuery.TagQuery.Has.Group, lookQuery.TagQuery.Has.Name)),
                        BooleanClause.Occur.MUST);
            }

            // Not
            if (lookQuery.TagQuery.Not != null)
            {
                parsingContext.QueryAdd(
                        new TermQuery(new Term(LookConstants.TagsField + lookQuery.TagQuery.Not.Group, lookQuery.TagQuery.Not.Name)),
                        BooleanClause.Occur.MUST_NOT);
            }

            // HasAll
            if (lookQuery.TagQuery.HasAll != null && lookQuery.TagQuery.HasAll.Any())
            {
                foreach (var tag in lookQuery.TagQuery.HasAll)
                {
                    parsingContext.QueryAdd(
                            new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                            BooleanClause.Occur.MUST);
                }
            }

            // HasAllOr
            if (lookQuery.TagQuery.HasAllOr != null && lookQuery.TagQuery.HasAllOr.Any() && lookQuery.TagQuery.HasAllOr.SelectMany(x => x).Any())
            {
                var orQuery = new BooleanQuery();

                foreach (var tagCollection in lookQuery.TagQuery.HasAllOr)
                {
                    if (tagCollection.Any())
                    {
                        var allTagQuery = new BooleanQuery();

                        foreach (var tag in tagCollection)
                        {
                            allTagQuery.Add(
                                new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                BooleanClause.Occur.MUST);
                        }

                        orQuery.Add(allTagQuery, BooleanClause.Occur.SHOULD);
                    }
                }

                parsingContext.QueryAdd(orQuery, BooleanClause.Occur.MUST);
            }

            // HasAny
            if (lookQuery.TagQuery.HasAny != null && lookQuery.TagQuery.HasAny.Any())
            {
                var anyTagQuery = new BooleanQuery();

                foreach (var tag in lookQuery.TagQuery.HasAny)
                {
                    anyTagQuery.Add(
                                    new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                    BooleanClause.Occur.SHOULD);
                }

                parsingContext.QueryAdd(anyTagQuery, BooleanClause.Occur.MUST);
            }

            // HasAnyAnd
            if (lookQuery.TagQuery.HasAnyAnd != null && lookQuery.TagQuery.HasAnyAnd.Any())
            {
                foreach (var tagCollection in lookQuery.TagQuery.HasAnyAnd)
                {
                    if (tagCollection.Any())
                    {
                        var anyTagQuery = new BooleanQuery();

                        foreach (var tag in tagCollection)
                        {
                            anyTagQuery.Add(
                                            new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                                            BooleanClause.Occur.SHOULD);
                        }

                        parsingContext.QueryAdd(anyTagQuery, BooleanClause.Occur.MUST);
                    }
                }
            }

            // NotAny
            if (lookQuery.TagQuery.NotAny != null && lookQuery.TagQuery.NotAny.Any())
            {
                foreach (var tag in lookQuery.TagQuery.NotAny)
                {
                    parsingContext.QueryAdd(
                        new TermQuery(new Term(LookConstants.TagsField + tag.Group, tag.Name)),
                        BooleanClause.Occur.MUST_NOT);
                }
            }            
        }
    }
}
