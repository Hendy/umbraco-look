using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        /// <summary>
        /// TODO: yield return
        /// </summary>
        /// <param name="publishedContent"></param>
        /// <returns></returns>
        internal static IEnumerable<IPublishedContent> GetFlatDetachedDescendants(this IPublishedContent publishedContent)
        {
            var detachedPublishedContent = new List<IPublishedContent>();

            var properties = publishedContent
                                .Properties
                                .Where(y => y.Value is IEnumerable<IPublishedContent>)
                                .Select(y => y.Value as IEnumerable<IPublishedContent>);

            foreach (var property in properties)
            {
                detachedPublishedContent.AddRange(property);
                
                foreach (var propertyItem in property)
                {
                    // recurse
                    detachedPublishedContent.AddRange(IPublishedContentExtensions.GetFlatDetachedDescendants(propertyItem));
                }
            }

            return detachedPublishedContent.ToArray();
        }
    }
}
