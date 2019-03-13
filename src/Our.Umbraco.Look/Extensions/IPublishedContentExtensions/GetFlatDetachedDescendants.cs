using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        /// <summary>
        /// For the supplied IPublishedContent, find all detached IPublishedContent exposed via properties
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static IPublishedContent[] GetFlatDetachedDescendants(this IPublishedContent item, List<IPublishedContent> flatDetachedItems = null)
        {
            flatDetachedItems = flatDetachedItems ?? new List<IPublishedContent>();

            if (item != null)
            {
                LogHelper.Debug(typeof(IPublishedContentExtensions), $"GetFlatDetachedDescendants() for Name = '{ item.Name }', Guid = '{ item.GetGuidKey().ToString() }'");

                var detachedItems = item
                                    .Properties
                                    .Where(x => x.Value is IEnumerable<IPublishedContent>)
                                    .Select(x => x.Value as IEnumerable<IPublishedContent>)
                                    .SelectMany(x => x)
                                    .Where(x => x != null)
                                    .Where(x => x.Id == 0) // ensure only detached items are added
                                    .Where(x => x.GetGuidKey() != Guid.Empty) // detached items must have a valid key
                                    .Where(x => !flatDetachedItems.Select(y => y.GetGuidKey()).Contains(x.GetGuidKey())) // safety check to prevent duplicates
                                    .ToArray();

                foreach (var detachedItem in detachedItems)
                {                    
                    flatDetachedItems.Add(detachedItem);

                    detachedItem.GetFlatDetachedDescendants(flatDetachedItems); // recurse and forget result (as collection passed back in and built-up)
                }
            }

            return flatDetachedItems.ToArray();
        }
    }
}
