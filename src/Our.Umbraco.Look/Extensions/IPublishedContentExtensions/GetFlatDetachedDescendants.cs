using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        /// <summary>
        /// For the supplied IPublishedContent, find all detached IPublishedContent collection exposed via properties
        /// TODO: yield return
        /// </summary>
        /// <param name="publishedContent"></param>
        /// <returns></returns>
        internal static IEnumerable<IPublishedContent> GetFlatDetachedDescendants(this IPublishedContent publishedContent)
        {
            var detachedPublishedContent = new List<IPublishedContent>();

            if (publishedContent != null)
            {
                var publishedContentProperties = publishedContent
                                                    .Properties
                                                    .Where(x => x.Value is IEnumerable<IPublishedContent>)
                                                    .Select(x => x.Value as IEnumerable<IPublishedContent>)
                                                    .SelectMany(x => x)
                                                    .Where(x => x != null)
                                                    .Where(x => x.Id == 0) // ensure only detached items are added
                                                    .Where(x => x.GetGuidKey() != Guid.Empty)
                                                    .ToArray();

                foreach (var childPublishedContent in publishedContentProperties)
                {                    
                    detachedPublishedContent.Add(childPublishedContent);

                    detachedPublishedContent.AddRange(childPublishedContent.GetFlatDetachedDescendants()); // recurse
                }
            }

            return detachedPublishedContent.ToArray();
        }
    }
}
