using System;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class MatchesResult
    {
        /// <summary>
        /// array of matched Lucene documents (flattened from a LookResult)
        /// </summary>
        public Match[] Matches { get; set; }

        /// <summary>
        /// Flag to use by lazy loader to know when to stop
        /// </summary>
        public bool MoreItems { get; set; } = false;

        /// <summary>
        /// Total number of results matching the query supplied(ignores any skip/take values)
        /// </summary>
        public int TotalItemCount { get; set; } = -1;


        /// <summary>
        /// Respesents a matched document in the indexed (subset of a LookMatch)
        /// this is for api serialzation
        /// </summary>
        public class Match
        {
            public Guid Key { get; set; }

            public string Name { get; set; }


            // id, key
            // content,media,member
            // tags

            // link
            // name
        }
    }
}
