using System;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        /// <summary>
        /// Return the found item or null
        /// </summary>
        /// <param name="item">The IPublishedContent item to start search from</param>
        /// <returns>The IPublishedContent matching the supplied key, or null</returns>
        internal static IPublishedContent GetDetachedDescendant(this IPublishedContent item, Guid key)
        {
               return IPublishedContentExtensions
                        .YieldFlatDetachedDescendants(item)
                        .FirstOrDefault(x => x.GetGuidKey() == key);
        }
    }
}
