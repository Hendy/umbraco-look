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
        [JsonProperty("matches")]
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
            [JsonProperty("score")]
            public float Score { get; set; }

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

            // proeprties to indicate if this match has values set for each type of indexer

            //[JsonProperty("hasName")]
            //public bool HasName { get; set; }

            //[JsonProperty("hasText")]
            //public bool HasText { get; set; }

            //[JsonProperty("hasDate")]
            //public bool HasDate { get; set; }

            //[JsonProperty("hasTags")]
            //public bool HasTags { get; set; }

            //[JsonProperty("hasLocation")]
            //public bool HasLocation { get; set; }

            /// <summary>
            /// #/content/content/edit/1074
            /// #/media/media/edit/1096
            /// #/member/member/edit/62b351b9-1dfe-41ab-9336-31fe72374d41
            /// </summary>
            [JsonProperty("link")]
            public string Link { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lookMatch"></param>
            public static explicit operator Match(LookMatch lookMatch)
            {
                var match = new Match();

                match.Score = lookMatch.Score;
                match.Id = lookMatch.Id;
                match.Key = lookMatch.Key;
                match.Name = lookMatch.Name;

                switch (lookMatch.PublishedItemType)
                {
                    case PublishedItemType.Content:
                        match.Icon = "icon-selection-traycontent";
                        match.Link = "#/content/content/edit/" + (lookMatch.IsDetached ? lookMatch.HostItem.Id : lookMatch.Item.Id);
                        break;

                    case PublishedItemType.Media:
                        match.Icon = "icon-selection-traymedia";
                        match.Link = "#/media/media/edit/" + (lookMatch.IsDetached ? lookMatch.HostItem.Id : lookMatch.Item.Id);
                        break;

                    case PublishedItemType.Member:
                        match.Icon = "icon-selection-traymember";
                        //match.Link = "#/member/member/edit/" + (lookMatch.IsDetached ? lookMatch.HostItem.Get : lookMatch.Key.ToString())
                        break;
                }

                match.IsDetached = lookMatch.IsDetached;
                

                return match;
            }
        }
    }
}
