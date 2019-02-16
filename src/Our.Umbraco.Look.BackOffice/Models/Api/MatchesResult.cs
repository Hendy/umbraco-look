using Newtonsoft.Json;
using System;
using System.Linq;
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

            [JsonProperty("icon")]
            public string Icon { get; set; }

            [JsonProperty("isDetached")]
            public bool IsDetached { get; set; }

            //[JsonProperty("hasName")]
            //public bool HasName { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            //[JsonProperty("hasDate")]
            //public bool HasDate { get; set; }

            [JsonProperty("date")]
            public DateTime? Date { get; set; }

            //[JsonProperty("hasTags")]
            //public bool HasTags { get; set; }

            [JsonProperty("tags")]
            public Tag[] Tags { get; set; }

            //[JsonProperty("hasText")]
            //public bool HasText { get; set; }

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
            /// Tag for api serialization
            /// </summary>
            public class Tag
            {
                [JsonProperty("group")]
                public string Group { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                /// <summary>
                /// internal back office link to tag node in look tree
                /// </summary>
                [JsonProperty("link")]
                public string Link { get; set; }
            }

            /// <summary>
            /// Map a LookMatch object (as returned by Look) to a Match object (used for back office view model rendering)
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
                        match.Icon = "icon-umb-content"; // "icon-selection-traycontent";
                        match.Link = "#/content/content/edit/" + (lookMatch.IsDetached ? lookMatch.HostItem.Id : lookMatch.Item.Id);
                        break;

                    case PublishedItemType.Media:
                        match.Icon = "icon-umb-media"; // "icon-selection-traymedia";
                        match.Link = "#/media/media/edit/" + (lookMatch.IsDetached ? lookMatch.HostItem.Id : lookMatch.Item.Id);
                        break;

                    case PublishedItemType.Member:
                        match.Icon = "icon-umb-members"; // "icon-selection-traymember";
                        //match.Link = "#/member/member/edit/" + (lookMatch.IsDetached ? lookMatch.HostItem.Get : lookMatch.Key.ToString())
                        break;
                }

                match.IsDetached = lookMatch.IsDetached;
                match.Date = lookMatch.Date;

                match.Tags = lookMatch
                                .Tags
                                .Select(x => new Tag()
                                {
                                    Group = x.Group,
                                    Name = x.Name,
                                    Link = "#/developer/lookTree/Tag/" + lookMatch.SearcherName + "|" + x.Group + "|" + x.Name
                                })
                                .ToArray();

                return match;
            }
        }
    }
}
