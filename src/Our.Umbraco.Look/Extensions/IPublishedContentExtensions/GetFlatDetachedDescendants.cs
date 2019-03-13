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
        /// For the supplied IPublishedContent, find all detached IPublishedContent collection exposed via properties
        /// </summary>
        /// <param name="publishedContent"></param>
        /// <returns></returns>
        internal static IPublishedContent[] GetFlatDetachedDescendants(this IPublishedContent publishedContent)
        {
            var detachedPublishedContent = new List<IPublishedContent>();

            if (publishedContent != null)
            {
                LogHelper.Debug(typeof(IPublishedContentExtensions), $"GetFlatDetachedDescendants() for Name = '{ publishedContent.Name }', Guid = '{ publishedContent.GetGuidKey().ToString() }'");

                var publishedContentProperties = publishedContent
                                                    .Properties
                                                    .Where(x => x.Value is IEnumerable<IPublishedContent>)
                                                    .Select(x => x.Value as IEnumerable<IPublishedContent>)
                                                    .SelectMany(x => x)
                                                    .Where(x => x != null)
                                                    .Where(x => x.Id == 0) // ensure only detached items are added
                                                    .Where(x => x.GetGuidKey() != Guid.Empty)
                                                    //.Where(x => !detachedPublishedContent.Select(y => y.GetGuidKey()).Contains(x.GetGuidKey())) // safety check to prevent duplicates
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
