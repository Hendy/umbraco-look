using Examine.LuceneEngine;
using Our.Umbraco.Look.Extensions;
using Our.Umbraco.Look.Services;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Hooks into all configured Exmaine Umbraco indexers, allowing Look to add additional fields
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] // hide from api intellisense
    public class HookIndexing : ApplicationEventHandler
    {
        /// <summary>
        /// Umbraco started
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var indexProviders = LookService.GetExamineIndexers(); // goes via service, so it can be user configured

            if (!indexProviders.Any())
            {
                LogHelper.Warn(typeof(LookService), "Unable to find any Umbraco Examine indexers to hook into");
            }
            else
            {
                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                LookService.Initialize(umbracoHelper);

                foreach (var indexProvider in indexProviders)
                {
                    indexProvider.DocumentWriting += (sender, e) => this.Indexer_DocumentWriting(sender, e, umbracoHelper, indexProvider.Name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="umbracoHelper"></param>
        /// <param name="indexerName"></param>
        private void Indexer_DocumentWriting(object sender, DocumentWritingEventArgs e, UmbracoHelper umbracoHelper, string indexerName)
        {
            IPublishedContent publishedContent = null;

            publishedContent = umbracoHelper.TypedContent(e.NodeId);

            if (publishedContent == null)
            {
                // attempt to get as media
                publishedContent = umbracoHelper.TypedMedia(e.NodeId);

                if (publishedContent == null)
                {
                    // attempt to get as member
                    publishedContent = umbracoHelper.SafeTypedMember(e.NodeId);
                }
            }

            if (publishedContent != null)
            {
                this.EnsureUmbracoContext();

                var indexingContext = new IndexingContext(null, publishedContent, indexerName);

                LookService.Index(indexingContext, e.Document);
            }
        }

        /// <summary>
        /// HACK: this indexing event is on a thread outside of HttpContext, and context is required to get the url from IPublishedContent
        /// </summary>
        private void EnsureUmbracoContext()
        {
            var dummyHttpContext = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("", "", new StringWriter())));

            UmbracoContext.EnsureContext(
                                dummyHttpContext,
                                ApplicationContext.Current,
                                new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                                UmbracoConfig.For.UmbracoSettings(),
                                UrlProviderResolver.Current.Providers,                                
                                true,
                                false);
        }
    }
}
