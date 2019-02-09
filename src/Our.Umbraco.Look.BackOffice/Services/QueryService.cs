using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Look.BackOffice.Services
{
    /// <summary>
    /// common queries used by both the tree and api
    /// </summary>
    internal static class QueryService
    {
        /// <summary>
        /// return all tag groups (todo return facet counts)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        internal static string[] GetTagGroups(string searcherName)
        {

            var tagGroups = new LookQuery(searcherName) { TagQuery = new TagQuery() }
                    .Search()
                    .Matches
                    .SelectMany(x => x.Tags.Select(y => y.Group))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();

            return tagGroups;
        }

        internal static object GetTags(string searcherName, string tagGroup)
        {
            return null;
        }
    }
}
