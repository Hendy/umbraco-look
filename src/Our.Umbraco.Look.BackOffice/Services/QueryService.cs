using System.Linq;

namespace Our.Umbraco.Look.BackOffice.Services
{
    /// <summary>
    /// common queries used by both the tree and api
    /// </summary>
    internal static class QueryService
    {
        /// <summary>
        /// get all groups in seacher
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        internal static string[] GetTagGroups(string searcherName) //TODO: change to Dictionary<string, int> (to assoicate a count)
        {
            // TODO: return useage count for each (facets)

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
        internal static LookTag[] GetTags(string searcherName, string tagGroup) //TODO: change to Dictionary<LookTag, int> (to assoicate a count)
        {
            // TODO: return useage count for each (facets)

            return new LookQuery(searcherName) { TagQuery = new TagQuery() }
                        .Search()
                        .Matches
                        .SelectMany(x => x.Tags.Where(y => y.Group == tagGroup))
                        .Distinct()
                        .OrderBy(x => x.Name)
                        .ToArray();
        }


        //internal static QueryResult GetMatches(string searcherName, string tagGroup, string tagName, string sort)
        //{


        //    return null;
        //}

    }
}
