using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class UmbracoHelperExtensions         
    {
        internal static IPublishedContent GetPublishedContent(this UmbracoHelper umbracoHelper, int id)
        {
            IPublishedContent publishedContent = null;

            publishedContent = umbracoHelper.TypedContent(id);

            if (publishedContent == null)
            {
                // fallback to attempting to get media
                publishedContent = umbracoHelper.TypedMedia(id);
            }

            if (publishedContent == null)
            {
                // fallback to attempting to get member
                try
                {
                    publishedContent = umbracoHelper.TypedMember(id);
                }
                catch
                {
                    // HACK: suppress error
                }
            }

            return publishedContent;
        }
    }
}
