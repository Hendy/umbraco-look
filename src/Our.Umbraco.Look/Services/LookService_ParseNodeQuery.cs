using Lucene.Net.Index;
using Lucene.Net.Search;
using Our.Umbraco.Look.Exceptions;
using Our.Umbraco.Look.Extensions;
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
        private static void ParseNodeQuery(LookQuery lookQuery, ParsingContext parsingContext)
        {
            if (lookQuery.NodeQuery != null)
            {
                parsingContext.QueryAdd(new TermQuery(new Term(LookConstants.HasNodeField, "1")), BooleanClause.Occur.MUST);

                // HasType
                if (lookQuery.NodeQuery.Type != null)
                {
                    parsingContext.QueryAdd(
                            new TermQuery(
                                new Term(LookConstants.NodeTypeField, lookQuery.NodeQuery.Type.ToString())),
                                BooleanClause.Occur.MUST);
                }

                // HasTypeAny
                if (lookQuery.NodeQuery.TypeAny != null && lookQuery.NodeQuery.TypeAny.Any())
                {
                    var nodeTypeQuery = new BooleanQuery();

                    foreach (var nodeType in lookQuery.NodeQuery.TypeAny)
                    {
                        nodeTypeQuery.Add(
                            new TermQuery(
                                new Term(LookConstants.NodeTypeField, nodeType.ToString())),
                                BooleanClause.Occur.SHOULD);
                    }

                    parsingContext.QueryAdd(nodeTypeQuery, BooleanClause.Occur.MUST);
                }

                // Detached
                switch (lookQuery.NodeQuery.DetachedQuery)
                {
                    case DetachedQuery.ExcludeDetached:

                        parsingContext.QueryAdd(
                                new TermQuery(new Term(LookConstants.IsDetachedField, "1")),
                                BooleanClause.Occur.MUST_NOT);

                        break;

                    case DetachedQuery.OnlyDetached:

                        parsingContext.QueryAdd(
                            new TermQuery(new Term(LookConstants.IsDetachedField, "1")),
                            BooleanClause.Occur.MUST);

                        break;
                }

                // HasCulture
                if (lookQuery.NodeQuery.Culture != null)
                {
                    parsingContext.QueryAdd(
                            new TermQuery(
                                new Term(LookConstants.CultureField, lookQuery.NodeQuery.Culture.LCID.ToString())),
                                BooleanClause.Occur.MUST);
                }

                // HasCultureAny
                if (lookQuery.NodeQuery.CultureAny != null && lookQuery.NodeQuery.CultureAny.Any())
                {
                    var nodeCultureQuery = new BooleanQuery();

                    foreach (var nodeCulture in lookQuery.NodeQuery.CultureAny)
                    {
                        nodeCultureQuery.Add(
                            new TermQuery(
                                new Term(LookConstants.CultureField, nodeCulture.LCID.ToString())),
                                BooleanClause.Occur.SHOULD);
                    }

                    parsingContext.QueryAdd(nodeCultureQuery, BooleanClause.Occur.MUST);
                }

                // HasAlias
                if (lookQuery.NodeQuery.Alias != null)
                {
                    parsingContext.QueryAdd(
                            new TermQuery(
                                new Term(LookConstants.NodeAliasField, lookQuery.NodeQuery.Alias.ToString())),
                                BooleanClause.Occur.MUST);
                }

                // HasAliasAny
                if (lookQuery.NodeQuery.AliasAny != null && lookQuery.NodeQuery.AliasAny.Any())
                {
                    var nodeAliasQuery = new BooleanQuery();

                    foreach (var typeAlias in lookQuery.NodeQuery.AliasAny)
                    {
                        nodeAliasQuery.Add(
                                        new TermQuery(
                                            new Term(LookConstants.NodeAliasField, typeAlias)),
                                            BooleanClause.Occur.SHOULD);
                    }

                    parsingContext.QueryAdd(nodeAliasQuery, BooleanClause.Occur.MUST);
                }

                // Ids
                if (lookQuery.NodeQuery.Ids != null && lookQuery.NodeQuery.Ids.Any())
                {
                    if (lookQuery.NodeQuery.NotIds != null)
                    {
                        var conflictIds = lookQuery.NodeQuery.Ids.Where(x => lookQuery.NodeQuery.NotIds.Contains(x));

                        if (conflictIds.Any())
                        {
                            throw new ParsingException($"Conflict in NodeQuery, Ids: '{ string.Join(",", conflictIds) }' are in both Ids and NotIds");
                        }
                    }

                    var idQuery = new BooleanQuery();

                    foreach (var id in lookQuery.NodeQuery.Ids)
                    {
                        idQuery.Add(
                                    new TermQuery(new Term(LookConstants.NodeIdField, id.ToString())),
                                    BooleanClause.Occur.SHOULD);
                    }

                    parsingContext.QueryAdd(idQuery, BooleanClause.Occur.MUST);
                }

                // Keys
                if (lookQuery.NodeQuery.Keys != null && lookQuery.NodeQuery.Keys.Any())
                {
                    if (lookQuery.NodeQuery.NotKeys != null)
                    {
                        var conflictKeys = lookQuery.NodeQuery.Keys.Where(x => lookQuery.NodeQuery.NotKeys.Contains(x));

                        if (conflictKeys.Any())
                        {
                            throw new ParsingException($"Conflict in NodeQuery, keys: '{ string.Join(",", conflictKeys) }' are in both Keys and NotKeys");
                        }
                    }

                    var keyQuery = new BooleanQuery();

                    foreach (var key in lookQuery.NodeQuery.Keys)
                    {
                        keyQuery.Add(
                                    new TermQuery(new Term(LookConstants.NodeKeyField, key.GuidToLuceneString())),
                                    BooleanClause.Occur.SHOULD);
                    }

                    parsingContext.QueryAdd(keyQuery, BooleanClause.Occur.MUST);
                }

                // NotId
                if (lookQuery.NodeQuery.NotId != null)
                {
                    parsingContext.QueryAdd(
                            new TermQuery(new Term(LookConstants.NodeIdField, lookQuery.NodeQuery.NotId.ToString())),
                            BooleanClause.Occur.MUST_NOT);
                }

                // NotIds
                if (lookQuery.NodeQuery.NotIds != null && lookQuery.NodeQuery.NotIds.Any())
                {
                    foreach (var exculudeId in lookQuery.NodeQuery.NotIds)
                    {
                        parsingContext.QueryAdd(
                                new TermQuery(new Term(LookConstants.NodeIdField, exculudeId.ToString())),
                                BooleanClause.Occur.MUST_NOT);
                    }
                }

                // NotKey
                if (lookQuery.NodeQuery.NotKey != null)
                {
                    parsingContext.QueryAdd(
                            new TermQuery(new Term(LookConstants.NodeKeyField, lookQuery.NodeQuery.NotKey.ToString())),
                            BooleanClause.Occur.MUST_NOT);
                }

                // NotKeys
                if (lookQuery.NodeQuery.NotKeys != null && lookQuery.NodeQuery.NotKeys.Any())
                {
                    foreach (var excludeKey in lookQuery.NodeQuery.NotKeys)
                    {
                        parsingContext.QueryAdd(
                                new TermQuery(new Term(LookConstants.NodeKeyField, excludeKey.GuidToLuceneString())),
                                BooleanClause.Occur.MUST_NOT);
                    }
                }
            }
        }
    }
}
