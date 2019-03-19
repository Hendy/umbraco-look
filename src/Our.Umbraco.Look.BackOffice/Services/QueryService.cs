using Our.Umbraco.Look.BackOffice.Models.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.BackOffice.Services
{
    /// <summary>
    /// common queries used by both the tree and api
    /// </summary>
    internal static class QueryService
    {
        /// <summary>
        /// Get a distinct collection of cultures
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        internal static CultureInfo[] GetCultures(string searcherName)
        {
            // TODO: change to only query for cultures setup in the current umbraco site
            
            return new LookQuery(searcherName)
                            {
                                NodeQuery = new NodeQuery() {  Type = PublishedItemType.Content },
                                //RawQuery = "Look_Culture:*"
                            }
                            .Search()
                            .Matches
                            .Select(x => x.CultureInfo)
                            .Distinct()
                            .ToArray();
        }

        /// <summary>
        /// get all tag groups in seacher
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        internal static string[] GetTagGroups(string searcherName) //TODO: change to Dictionary<string, int> (to assoicate a count)
        {
            // TODO: return useage count for each (need new field)

            return new LookQuery(searcherName) { TagQuery = new TagQuery() }
                        .Search()
                        .Matches
                        .SelectMany(x => x.Tags.Select(y => y.Group))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToArray();
        }

        /// <summary>
        /// get all tags in group and gives each a count
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="tagGroup"></param>
        /// <returns></returns>
        internal static Dictionary<LookTag, int> GetTags(string searcherName, string tagGroup) //TODO: change to Dictionary<LookTag, int> (to assoicate a count)
        {
            return new LookQuery(searcherName) { TagQuery = new TagQuery() { FacetOn = new TagFacetQuery(tagGroup) } }
                                .Search()
                                .Facets
                                .Select(x => new Tuple<LookTag, int>(x.Tags.Single(), x.Count))
                                .OrderBy(x => x.Item1.Name)
                                .ToDictionary(x => x.Item1, x => x.Item2);
        }

        #region Get Matches

        /// <summary>
        /// get a chunk of matches
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        internal static MatchesResult GetMatches(string searcherName, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName);

            lookQuery.NodeQuery = new NodeQuery();

            QueryService.SetSort(lookQuery, sort);

            var lookResult = lookQuery.Search();

            matchesResult.TotalItemCount = lookResult.TotalItemCount;
            matchesResult.Matches = lookResult
                                        .SkipMatches(skip)
                                        .Take(take)
                                        .Select(x => (MatchesResult.Match)x) // convert match to model for serialization
                                        .ToArray();

            return matchesResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        internal static MatchesResult GetNodeTypeMatches(string searcherName, PublishedItemType nodeType, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName);

            lookQuery.NodeQuery = new NodeQuery()
            {
                Type = nodeType
            };

            QueryService.SetSort(lookQuery, sort);

            var lookResult = lookQuery.Search();

            matchesResult.TotalItemCount = lookResult.TotalItemCount;
            matchesResult.Matches = lookResult
                                        .Matches
                                        .Skip(skip)
                                        .Take(take)
                                        .Select(x => (MatchesResult.Match)x)
                                        .ToArray();

            return matchesResult;
        }

        /// <summary>
        /// Get matches by culture - all content has a culture set in Umbraco
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="lcid"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        internal static MatchesResult GetCultureMatches(string searcherName, int? lcid, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName) { NodeQuery = new NodeQuery() };

            if (lcid.HasValue)
            {
                lookQuery.NodeQuery.Culture = new CultureInfo(lcid.Value);
            }
            else // no culture suppled, so get all content
            {
                lookQuery.NodeQuery.Type = PublishedItemType.Content;
            }
            
            QueryService.SetSort(lookQuery, sort);

            var lookResult = lookQuery.Search();

            matchesResult.TotalItemCount = lookResult.TotalItemCount;
            matchesResult.Matches = lookResult
                                        .Matches
                                        .Skip(skip)
                                        .Take(take)
                                        .Select(x => (MatchesResult.Match)x)
                                        .ToArray();

            return matchesResult;
        }

        /// <summary>
        /// get a chunk of matches
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="tagGroup"></param>
        /// <param name="tagName"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        internal static MatchesResult GetTagMatches(string searcherName, string tagGroup, string tagName, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName);
            var tagQuery = new TagQuery(); // setting a tag query, means only items that have tags will be returned

            if (!string.IsNullOrWhiteSpace(tagGroup) && string.IsNullOrWhiteSpace(tagName)) // only have the group to query
            {
                // TODO: add a new field into the lucene index (would avoid additional query to first look up the tags in this group)
                var tagsInGroup = QueryService.GetTags(searcherName, tagGroup).Select(x => x.Key).ToArray();

                tagQuery.HasAny = tagsInGroup;

            }
            else if (!string.IsNullOrWhiteSpace(tagName)) // we have a specifc tag
            {
                tagQuery.Has = new LookTag(tagGroup, tagName);
            }

            lookQuery.TagQuery = tagQuery;

            QueryService.SetSort(lookQuery, sort);

            var lookResult = lookQuery.Search();

            matchesResult.TotalItemCount = lookResult.TotalItemCount;
            matchesResult.Matches = lookResult
                                        .SkipMatches(skip)
                                        .Take(take)
                                        .Select(x => (MatchesResult.Match)x)
                                        .ToArray();

            return matchesResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        internal static MatchesResult GetLocationMatches(string searcherName, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName);

            lookQuery.LocationQuery = new LocationQuery();

            QueryService.SetSort(lookQuery, sort);

            var lookResult = lookQuery.Search();

            matchesResult.TotalItemCount = lookResult.TotalItemCount;
            matchesResult.Matches = lookResult
                                        .Matches
                                        .Skip(skip)
                                        .Take(take)
                                        .Select(x => (MatchesResult.Match)x)
                                        .ToArray();

            return matchesResult;
        }

        #endregion

        private static void SetSort(LookQuery lookQuery, string sort)
        {
            switch (sort)
            {
                case "Score": lookQuery.SortOn = SortOn.Score; break;
                case "Name": lookQuery.SortOn = SortOn.Name; break;
                case "DateAscending": lookQuery.SortOn = SortOn.DateAscending; break;
                case "DateDescending": lookQuery.SortOn = SortOn.DateDescending; break;
            }
        }
    }
}
