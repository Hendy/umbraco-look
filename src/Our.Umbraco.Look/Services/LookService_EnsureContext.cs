using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Flag used to determine whether the http context has been ensured for the current thread
        /// </summary>
        [ThreadStatic]
        private static bool ContextEnsured = false;

        /// <summary>
        /// Ensure there is an HttpContext for the current thread
        /// </summary>
        internal static void EnsureContext()
        {
            if (!LookService.ContextEnsured)
            {
                var httpContext = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("", "", new StringWriter())));

                UmbracoContext.EnsureContext(
                                    httpContext,
                                    ApplicationContext.Current,
                                    new WebSecurity(httpContext, ApplicationContext.Current),
                                    UmbracoConfig.For.UmbracoSettings(),
                                    UrlProviderResolver.Current.Providers,
                                    true,
                                    false);

                LookService.ContextEnsured = true;
            }
        }
    }
}
