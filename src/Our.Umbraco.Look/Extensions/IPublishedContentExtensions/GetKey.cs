using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IPublishedContentExtensions
    {
        internal static Guid GetKey(this IPublishedContent publishedContent)
        {


         // https://our.umbraco.com/forum/extending-umbraco-and-using-the-api/85776-umbraco-761-ipublishedcontentgetkey-returns-empty-guid


            return Guid.Empty;
        }
    }
}
