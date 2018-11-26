using System;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class UmbracoHelperExtensions         
    {
        internal static IPublishedContent SafeTypedMember(this UmbracoHelper umbracoHelper, int id)
        {
            IPublishedContent publishedContent = null;

            try
            {
                publishedContent = umbracoHelper.TypedMember(id);
            }
            catch (Exception exception)
            {
                LogHelper.WarnWithException(typeof(UmbracoHelperExtensions), "Failed to get member by id", exception);
            }

            return publishedContent;
        }
    }
}
