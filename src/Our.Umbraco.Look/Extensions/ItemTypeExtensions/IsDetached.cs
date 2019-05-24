using System;

namespace Our.Umbraco.Look.Extensions
{
    public static partial class ItemTypeExtensions
    {
        /// <summary>
        /// Determine if the supplied Look ItemItem enum represents a detached item
        /// </summary>
        /// <param name="itemType">The Look ItemType</param>
        /// <returns>True if the Look ItemType is detached</returns>
        public static bool IsDetached(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.DetachedContent:
                case ItemType.DetachedMedia:
                case ItemType.DetachedMember:
                    return true;

                case ItemType.Content:
                case ItemType.Media:
                case ItemType.Member:
                    return false;
            }

            throw new Exception($"Unexpected ItemType value: {itemType}");
        }
    }
}
