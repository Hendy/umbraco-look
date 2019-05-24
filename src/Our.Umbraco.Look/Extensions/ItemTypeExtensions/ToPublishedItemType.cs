using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    public static partial class ItemTypeExtensions
    {
        /// <summary>
        /// Get the Umbraco enum PublishedItemType corresponding to the Look ItemType (detached items return the type of their host)
        /// </summary>
        /// <param name="itemType">The Look ItemType</param>
        /// <returns>The corresponfing Umbraco PublishedItemType</returns>
        public static PublishedItemType ToPublishedItemType(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Content:
                case ItemType.DetachedContent:
                    return PublishedItemType.Content;

                case ItemType.Media:
                case ItemType.DetachedMedia:
                    return PublishedItemType.Media;

                case ItemType.Member:
                case ItemType.DetachedMember:
                    return PublishedItemType.Member;
            }

            throw new Exception($"Unexpected ItemType value: {itemType}");
        }
    }
}
