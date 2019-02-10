using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Look.BackOffice.Services
{
    /// <summary>
    /// common queries used by both the tree and api
    /// </summary>
    internal static class QueryService
    {
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


        //internal static QueryResult GetMatches(string searcherName, string tagGroup, string tagName, string sort)
        //{


        //    return null;
        //}

    }
}
