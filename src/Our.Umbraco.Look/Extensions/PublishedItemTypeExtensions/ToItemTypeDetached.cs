using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    public static partial class PublishedItemTypeExtensions
    {
        /// <summary>
        /// Helper to convert from an Umbraco PublishedItemType enum value to a detached version of a Look ItemType
        /// </summary>
        /// <param name="publishedItemType">The PublishedItemType value to convert from</param>
        /// <returns>The corresponding detached ItemType value</returns>
        public static ItemType ToItemTypeDetached(this PublishedItemType publishedItemType)
        {
            switch (publishedItemType)
            {
                case PublishedItemType.Content: return ItemType.DetachedContent;
                case PublishedItemType.Media: return ItemType.DetachedMedia;
                case PublishedItemType.Member: return ItemType.DetachedMember;
            }

            throw new Exception("Unknown PublishedItemType");
        }
    }
}
