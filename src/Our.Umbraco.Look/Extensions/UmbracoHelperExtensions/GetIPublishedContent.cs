using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class UmbracoHelperExtensions         
    {
        /// <summary>
        /// Helper to inflate IPublishedContent item from Umbraco id
        /// </summary>
        /// <param name="umbracoHelper"></param>
        /// <param name="id"></param>
        /// <returns>IPublishedContent if found, otherwize null</returns>
        internal static IPublishedContent GetIPublishedContent(this UmbracoHelper umbracoHelper, int id)
        {
            if (id <= 0) return null;

            var publishedContent = umbracoHelper.TypedContent(id);

            if (publishedContent == null)
            {
                publishedContent = umbracoHelper.TypedMedia(id);
            }

            if (publishedContent == null)
            {
                publishedContent = umbracoHelper.SafeTypedMember(id);
            }

            return publishedContent;
        }
    }
}
