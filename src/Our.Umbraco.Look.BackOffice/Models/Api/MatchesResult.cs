﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Our.Umbraco.Look.BackOffice.Services;
using Our.Umbraco.Look.Extensions;
using System;
using System.Linq;
using Umbraco.Web;
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

            /// <summary>
            /// alias of the docType/mediaType/memberType
            /// </summary>
            [JsonProperty("alias")]
            public string Alias { get; set; }

            /// <summary>
            /// The icon of the docType/mediaType/memberType alias
            /// </summary>
            [JsonProperty("aliasIcon")]
            public string AliasIcon { get; set; }

            [JsonProperty("type")]
            [JsonConverter(typeof(StringEnumConverter))]
            public ItemType Type { get; set; }

            /// <summary>
            /// icon representing the type (content/media/member) and whether it's detached - TODO: rename to TypeIcon
            /// </summary>
            [JsonProperty("icon")]
            public string Icon { get; set; }

            [JsonProperty("isDetached")]
            public bool IsDetached { get; set; }

            [JsonProperty("culture")]
            public string Culture { get; set; }

            //[JsonProperty("hasName")]
            //public bool HasName => !string.IsNullOrWhiteSpace(this.Name);

            [JsonProperty("name")]
            public string Name { get; set; }

            //[JsonProperty("hasDate")]
            //public bool HasDate { get; set; }

            [JsonProperty("date")]
            public DateTime? Date { get; set; }

            [JsonProperty("hasText")]
            public bool HasText { get; set; }

            [JsonProperty("hasTags")]
            public bool HasTags { get; set; }

            [JsonProperty("tagGroups")]
            public TagGroup[] TagGroups { get; set; }

            [JsonProperty("hasLocation")]
            public bool HasLocation { get; set; }

            [JsonProperty("latitide")]
            public double? Latitude { get; set; }

            [JsonProperty("longitude")]
            public double? Longitude { get; set; }

            /// <summary>
            /// names of all ancestors of this content / media
            /// (string rendered before the link text)
            /// </summary>
            [JsonProperty("path")]
            public string Path { get; set; }

            /// <summary>
            /// The name of the Umbraco item
            /// </summary>
            [JsonProperty("pathItem")]
            public string PathItem { get; set; }

            /// <summary>
            /// #/content/content/edit/1074
            /// #/media/media/edit/1096
            /// #/member/member/edit/62b351b9-1dfe-41ab-9336-31fe72374d41
            /// </summary>
            [JsonProperty("link")]
            public string Link { get; set; }

            /// <summary>
            /// This is the umbraco human friendly path to the content / media or member
            /// </summary>
            [JsonProperty("linkText")]
            public string LinkText { get; set; }

            /// <summary>
            /// Tag Group for api serialization
            /// </summary>
            public class TagGroup
            {
                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("link")]
                public string Link { get; set; }

                [JsonProperty("tags")]
                public Tag[] Tags { get; set; }
            }

            /// <summary>
            /// Tag for api serialization
            /// </summary>
            public class Tag
            {
                /// <summary>
                /// property used as a helper for sorting, no need to serialize as this tag will be inside a TagGroup obj
                /// </summary>
                [JsonIgnore]
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
                match.Alias = lookMatch.Item.DocumentTypeAlias;
                match.AliasIcon = IconService.GetAliasIcon(lookMatch.ItemType.ToPublishedItemType(), lookMatch.ItemType.IsDetached(), lookMatch.Alias);
                match.Type = lookMatch.ItemType;
                match.Culture = lookMatch.CultureInfo?.Name;
                match.Name = lookMatch.Name;

                var item = lookMatch.ItemType.IsDetached() ? lookMatch.HostItem : lookMatch.Item; // set it to be the non-detached
                var pathIds = item
                                .Path
                                .Split(',')
                                .Select(x => int.Parse(x))
                                .Where(x => x > 0 && x != item.Id)
                                .ToArray();

                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                switch (lookMatch.ItemType.ToPublishedItemType())
                {
                    case PublishedItemType.Content:                        
                        match.Icon = "icon-umb-content";
                        match.Path = pathIds.Any() ? string.Join("\\", pathIds.Select(x => umbracoHelper.TypedContent(x).Name)) + "\\" : string.Empty;
                        match.PathItem = item.Name;
                        match.Link = "#/content/content/edit/" + item.Id;
                        match.LinkText = item.Name;
                        break;

                    case PublishedItemType.Media:
                        match.Icon = "icon-umb-media";
                        match.Path = pathIds.Any() ? string.Join("\\", pathIds.Select(x => umbracoHelper.TypedMedia(x).Name)) + "\\" : string.Empty;
                        match.PathItem = item.Name;
                        match.Link = "#/media/media/edit/" + (lookMatch.ItemType.IsDetached() ? lookMatch.HostItem.Id : lookMatch.Item.Id);
                        match.LinkText = item.Name;
                        break;

                    case PublishedItemType.Member:
                        match.Icon = "icon-umb-members"; // "icon-selection-traymember";
                        match.Path = "";
                        match.PathItem = item.Name;
                        //match.Link = "#/member/member/edit/" + (lookMatch.IsDetached ? lookMatch.HostItem.Get : lookMatch.Key.ToString())
                        match.LinkText = item.Name;
                        break;
                }

                match.IsDetached = lookMatch.ItemType.IsDetached();
                match.Date = lookMatch.Date;

                match.HasText = !string.IsNullOrWhiteSpace(lookMatch.Text);

                var tags = lookMatch
                                .Tags
                                .Select(x => new Tag()
                                {
                                    Group = x.Group,
                                    Name = x.Name,
                                    Link = "#/developer/lookTree/Tag/" + lookMatch.SearcherName + "|" + x.Group + "|" + x.Name
                                })
                                .ToArray();

                match.HasTags = tags.Any();

                match.TagGroups = tags
                                    .Select(x => x.Group)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .Select(x => new TagGroup()
                                    {
                                        Name = x,
                                        Link = "#/developer/lookTree/TagGroup/" + lookMatch.SearcherName + "|" + x,
                                        Tags = tags.Where(y => y.Group == x).OrderBy(y => y.Name).ToArray()
                                    })
                                    .ToArray();


                match.HasLocation = lookMatch.Location != null;
                match.Latitude = lookMatch?.Location?.Latitude;
                match.Longitude = lookMatch?.Location?.Longitude;

                return match;
            }
        }
    }
}
