using Our.Umbraco.Look.BackOffice.Models.Api;
using Our.Umbraco.Look.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
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
            var cultures = new List<CultureInfo>();

            var languages = ApplicationContext.Current?.Services.LocalizationService.GetAllLanguages().ToArray();

            if (languages != null && languages.Any())
            {
                var lookQuery = new LookQuery(searcherName) { NodeQuery = new NodeQuery() };

                foreach(var culture in languages.Select(x => x.CultureInfo))
                {
                    lookQuery.NodeQuery.Culture = culture;

                    if (lookQuery.Search().TotalItemCount > 0)
                    {
                        cultures.Add(culture);
                    }
                }
            }

            return cultures.ToArray();
        }

        /// <summary>
        /// get all tag groups in seacher
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        internal static string[] GetTagGroups(string searcherName)
        {
            return new LookQuery(searcherName) { TagQuery = new TagQuery() }
                        .Search()
                        .Matches
                        .SelectMany(x => x.Tags.Select(y => y.Group))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToArray();
        }

        /// <summary>
        /// get all tags in group
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="tagGroup"></param>
        /// <returns></returns>
        internal static string[] GetTagNames(string searcherName, string tagGroup)
        {
            return new LookQuery(searcherName)
                            {
                                TagQuery = new TagQuery(),
                                RawQuery = "Look_TagGroup_" + tagGroup + ":1"
                            }
                            .Search()
                            .Matches
                            .SelectMany(x => x.Tags.Where(y => y.Group == tagGroup))
                            .Select(x => x.Name)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToArray();
        }

        #region Get Filters

        internal static FiltersResult GetFilters(string searcherName)
        {
            var filtersResult = new FiltersResult();

            // TODO: extend Look to have a public NodeFacetQuery (this will facet on culture, alias and type, returning counts for each) ?

            filtersResult.Aliases = new LookQuery(searcherName) { NodeQuery = new NodeQuery() }
                                        .Search()
                                        .Matches
                                        .Select(x => x.Alias)
                                        .Distinct()
                                        .OrderBy(x => x)
                                        .ToArray();

            return filtersResult;
        }

        internal static FiltersResult GetNodeTypeFilters(string searcherName, PublishedItemType nodeType)
        {
            var filtersResult = new FiltersResult();

            filtersResult.Aliases = new LookQuery(searcherName) { NodeQuery = new NodeQuery() { Type = nodeType.ToItemType() } }
                            .Search()
                            .Matches
                            .Select(x => x.Alias)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToArray();

            return filtersResult;
        }

        internal static FiltersResult GetDetachedFilters(string searcherName, PublishedItemType nodeType)
        {
            var filtersResult = new FiltersResult();

            filtersResult.Aliases = new LookQuery(searcherName) { NodeQuery = new NodeQuery() { Type = nodeType.ToItemType(), DetachedQuery = DetachedQuery.OnlyDetached } }
                            .Search()
                            .Matches
                            .Select(x => x.Alias)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToArray();

            return filtersResult;
        }

        internal static FiltersResult GetTagFilters(string searcherName, string tagGroup, string tagName)
        {
            var filtersResult = new FiltersResult();

            var lookQuery = new LookQuery(searcherName) { TagQuery = new TagQuery() };

            if (!string.IsNullOrWhiteSpace(tagGroup) && string.IsNullOrWhiteSpace(tagName)) // only have the group to query
            {
                //use raw query looking for newly indexed field(in look 0.33.0)
                // TODO: update look to handle tags like "colour:*"
                lookQuery.RawQuery = "Look_TagGroup_" + tagGroup + ":1";
            }
            else if (!string.IsNullOrWhiteSpace(tagName)) // we have a specifc tag
            {
                lookQuery.TagQuery.Has = new LookTag(tagGroup, tagName);
            }

            filtersResult.Aliases = lookQuery
                                    .Search()
                                    .Matches
                                    .Select(x => x.Alias)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToArray();

            return filtersResult;
        }

        internal static FiltersResult GetLocationFilters(string searcherName)
        {
            var filtersResult = new FiltersResult();

            filtersResult.Aliases = new LookQuery(searcherName) { LocationQuery = new LocationQuery() }
                            .Search()
                            .Matches
                            .Select(x => x.Alias)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToArray();

            return filtersResult;
        }

        #endregion

        #region Get Matches

        /// <summary>
        /// get a chunk of matches
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        internal static MatchesResult GetMatches(string searcherName, string filter, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName);

            lookQuery.NodeQuery = new NodeQuery() { Alias = !string.IsNullOrWhiteSpace(filter) ? filter : null };

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
        internal static MatchesResult GetNodeTypeMatches(string searcherName, PublishedItemType nodeType, string filter, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName);

            lookQuery.NodeQuery = new NodeQuery()
            {
                Type = nodeType.ToItemType(),
                Alias = !string.IsNullOrWhiteSpace(filter) ? filter : null
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
        /// 
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="nodeType"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        internal static MatchesResult GetDetachedMatches(string searcherName, PublishedItemType nodeType, string filter, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName)
                            {
                                NodeQuery = new NodeQuery()
                                {
                                    Type = nodeType.ToItemType(),
                                    Alias = !string.IsNullOrWhiteSpace(filter) ? filter : null,
                                    DetachedQuery = DetachedQuery.OnlyDetached
                                }
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
        /// get a chunk of matches
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="tagGroup"></param>
        /// <param name="tagName"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        internal static MatchesResult GetTagMatches(string searcherName, string tagGroup, string tagName, string filter, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName) { TagQuery = new TagQuery() };

            if (!string.IsNullOrWhiteSpace(tagGroup) && string.IsNullOrWhiteSpace(tagName)) // only have the group to query
            {
                //use raw query looking for newly indexed field(in look 0.33.0)
                // TODO: update look to handle tags like "colour:*"
                lookQuery.RawQuery = "Look_TagGroup_" + tagGroup + ":1"; 
            }
            else if (!string.IsNullOrWhiteSpace(tagName)) // we have a specifc tag
            {
                lookQuery.TagQuery.Has = new LookTag(tagGroup, tagName);
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                lookQuery.NodeQuery = new NodeQuery() { Alias = filter };
            }

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
        internal static MatchesResult GetLocationMatches(string searcherName, string filter, string sort, int skip, int take)
        {
            var matchesResult = new MatchesResult();

            var lookQuery = new LookQuery(searcherName);

            lookQuery.LocationQuery = new LocationQuery();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                lookQuery.NodeQuery = new NodeQuery() { Alias = filter };
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

        #endregion

        private static void SetSort(LookQuery lookQuery, string sort)
        {
            switch (sort)
            {
                case "Score": lookQuery.SortOn = SortOn.Score; break;
                case "Name": lookQuery.SortOn = SortOn.Name; break;
                case "Date": lookQuery.SortOn = SortOn.DateDescending; break;
            }
        }
    }
}
