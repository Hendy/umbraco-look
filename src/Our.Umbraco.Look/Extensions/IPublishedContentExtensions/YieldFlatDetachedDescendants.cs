using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        /// <summary>
        /// For the supplied IPublishedContent item, recurse all of its properties that return collections of IPublishedContent items
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static IEnumerable<IPublishedContent> YieldFlatDetachedDescendants(IPublishedContent item)
        {
            if (item != null)
            {
                var detachedItems = item
                    .Properties
                    .Where(x => x.Value is IEnumerable<IPublishedContent>)
                    .Select(x => x.Value as IEnumerable<IPublishedContent>)
                    .SelectMany(x => x)
                    .Where(x => x != null)
                    .Where(x => x.Id == 0) // ensure only detached items are added
                    .Where(x => x.GetGuidKey() != Guid.Empty); // detached items must have a valid key

                foreach (var detachedItem in detachedItems)
                {
                    yield return detachedItem;
                }
            }
        }
    }
}
