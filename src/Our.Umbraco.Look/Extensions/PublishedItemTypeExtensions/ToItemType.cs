using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    public static partial class PublishedItemTypeExtensions
    {
        /// <summary>
        /// Helper to convert from an Umbraco PublishedItemType enum value to a local enum value
        /// </summary>
        /// <param name="publishedItemType">The PublishedItemType value to convert from</param>
        /// <param name="detached">Optional flag to indicate if the detached version should be returned (defaults to false)</param>
        /// <returns>The corresponding ItemType value</returns>
        public static ItemType ToItemType(this PublishedItemType publishedItemType, bool detached = false)
        {
            if (!detached)
            {
                switch (publishedItemType)
                {
                    case PublishedItemType.Content: return ItemType.Content;
                    case PublishedItemType.Media: return ItemType.Media;
                    case PublishedItemType.Member: return ItemType.Member;
                }
            }
            else
            {
                switch (publishedItemType)
                {
                    case PublishedItemType.Content: return ItemType.DetachedContent;
                    case PublishedItemType.Media: return ItemType.DetachedMedia;
                    case PublishedItemType.Member: return ItemType.DetachedMember;
                }
            }

            throw new Exception("Unkown PublishedItemType");
        }
    }
}
