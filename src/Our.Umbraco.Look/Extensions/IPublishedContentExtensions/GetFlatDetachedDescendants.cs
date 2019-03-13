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
        /// For the supplied IPublishedContent item, recurse all of its properties that return collections of IPublishedContent items
        /// Return an array of all distinct detached (by guid key) IPublishedContent items
        /// </summary>
        /// <param name="item">The IPublishedContent item to get all detached IPublihedContent items for</param>
        /// <param name="flatDetachedItems">The List into which to add the detached IPublishedContent items</param>
        /// <returns>All detached IPublishedContent items as a flat Array</returns>
        internal static IPublishedContent[] GetFlatDetachedDescendants(this IPublishedContent item, List<IPublishedContent> flatDetachedItems = null)
        {
            flatDetachedItems = flatDetachedItems ?? new List<IPublishedContent>();

            if (item != null)
            {
                LogHelper.Debug(typeof(IPublishedContentExtensions), $"GetFlatDetachedDescendants() for Name = '{ item.Name }', Key = '{ item.GetGuidKey() }'");

                var detachedItems = item
                                    .Properties
                                    .Where(x => x.Value is IEnumerable<IPublishedContent>)
                                    .Select(x => x.Value as IEnumerable<IPublishedContent>)
                                    .SelectMany(x => x)
                                    .Where(x => x != null)
                                    .Where(x => x.Id == 0) // ensure only detached items are added
                                    .Where(x => x.GetGuidKey() != Guid.Empty) // detached items must have a valid key

                                    // safety check to prevent duplicates
                                    .Where(x => !flatDetachedItems.Select(y => y.GetGuidKey()).Contains(x.GetGuidKey())) 
                                    .ToArray();

                foreach (var detachedItem in detachedItems)
                {                    
                    flatDetachedItems.Add(detachedItem);

                    // recurse and ignore result (as flatDetachedItems list is added to)
                    detachedItem.GetFlatDetachedDescendants(flatDetachedItems); 
                }
            }

            return flatDetachedItems.ToArray();
        }
    }
}
