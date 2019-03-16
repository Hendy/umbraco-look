using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        private static IEnumerable<IPublishedContent> YieldDetachedDescendants(this IPublishedContent item)
        {
            if (item != null)
            {
                foreach (var detachedProperty in IPublishedContentExtensions.YieldDetachedProperties(item))
                {
                    yield return detachedProperty;

                    foreach (var detachedPropertyDescendant in IPublishedContentExtensions.YieldDetachedDescendants(detachedProperty))
                    {
                        yield return detachedPropertyDescendant;
                    }
                }
            }
        }
    }
}
