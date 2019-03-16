using System;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        /// <summary>
        /// First a detached IPublishedContent item by key
        /// </summary>
        /// <param name="item">The IPublishedContent item to start search from</param>
        /// <returns>The IPublishedContent matching the supplied key, or null</returns>
        internal static IPublishedContent GetDetachedDescendant(this IPublishedContent item, Guid key)
        {
            return IPublishedContentExtensions
                    .YieldDetachedDescendants(item)
                    .FirstOrDefault(x => x.GetGuidKey() == key);

        }
    }

}
