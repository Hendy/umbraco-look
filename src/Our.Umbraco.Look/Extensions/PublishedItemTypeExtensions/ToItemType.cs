using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class PublishedItemTypeExtensions
    {
        /// <summary>
        /// Helper to convert from an Umbraco PublishedItemType enum value to a local enum value
        /// </summary>
        /// <param name="value">The PublishedItemType value to convert from</param>
        /// <returns>The corresponding ItemType value</returns>
        internal static ItemType ToItemType(this PublishedItemType publishedItemType)
        {
            switch(publishedItemType)
            {
                case PublishedItemType.Content: return ItemType.Content;
                case PublishedItemType.Media: return ItemType.Media;
                case PublishedItemType.Member: return ItemType.Member;
            }

            throw new Exception("Unkown PublishedItemType");
        }
    }
}
