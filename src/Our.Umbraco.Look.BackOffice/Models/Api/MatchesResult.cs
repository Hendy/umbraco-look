using Newtonsoft.Json;
using System;

using PublishedItemType = Umbraco.Core.Models.PublishedItemType;

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
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("key")]
            public Guid Key { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("icon")]
            public string Icon { get; set; }

            [JsonProperty("isDetached")]
            public bool IsDetached { get; set; }

            // Link

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lookMatch"></param>
            public static explicit operator Match(LookMatch lookMatch)
            {
                var match = new Match();

                match.Id = lookMatch.Id;
                match.Key = lookMatch.Key;
                match.Name = lookMatch.Name;

                switch (lookMatch.PublishedItemType)
                {
                    case PublishedItemType.Content: match.Icon = "icon-selection-traycontent"; break;
                    case PublishedItemType.Media: match.Icon = "icon-selection-traymedia"; break;
                    case PublishedItemType.Member: match.Icon = "icon-selection-traymember"; break;
                }

                match.IsDetached = lookMatch.IsDetached;


                return match;
            }
        }
    }
}
