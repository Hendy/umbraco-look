using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        /// <summary>
        /// For the supplied IPublishedContent item, recurse all of its properties that return collections of IPublishedContent items (doing a safety check to ensure duplicates are not returned)
        /// Return an array of all distinct detached (by guid key) IPublishedContent items
        /// </summary>
        /// <param name="item">The IPublishedContent item to get all detached IPublihedContent items for</param>
        /// <param name="flatDetachedItems">The List into which to add the detached IPublishedContent items</param>
        /// <returns>All detached IPublishedContent items as a flat Array</returns>
        internal static IPublishedContent[] GetDetachedDescendants(this IPublishedContent item)
        {
            var enumerator = IPublishedContentExtensions.YieldDetachedDescendants(item).GetEnumerator();

            var items = new List<IPublishedContent>();
            var ok = false;

            if (enumerator.MoveNext())
            {
                do
                {
                    var detachedItem = enumerator.Current;

                    ok = !items.Any(x => x.GetGuidKey() == detachedItem.GetGuidKey());

                    if (ok)
                    {
                        items.Add(detachedItem);
                    }

                } while (enumerator.MoveNext() && ok);
            }

            return items.ToArray();
        }
    }
}
