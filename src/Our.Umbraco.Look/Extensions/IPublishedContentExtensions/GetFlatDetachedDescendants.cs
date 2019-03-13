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

            var publishedContentCollections = publishedContent
                                                .Properties
                                                .Where(y => y.Value is IEnumerable<IPublishedContent>)
                                                .Select(y => y.Value as IEnumerable<IPublishedContent>);

            foreach (var publishedContentCollection in publishedContentCollections)
            {
                // ensure only detached items are added
                detachedPublishedContent.AddRange(publishedContentCollection.Where(x => x.Id == 0)); // TODO: ensure not null & key a valid non-empty guid
                
                foreach (var childPublishedContent in publishedContentCollection)
                {
                    // recurse
                    detachedPublishedContent.AddRange(IPublishedContentExtensions.GetFlatDetachedDescendants(childPublishedContent));
                }
            }

            return detachedPublishedContent.ToArray();
        }
    }
}
