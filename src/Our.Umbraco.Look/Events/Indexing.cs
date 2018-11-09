using Examine.LuceneEngine;
using Our.Umbraco.Look.Extensions;
using Our.Umbraco.Look.Models;
using Our.Umbraco.Look.Services;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Our.Umbraco.Look.Events
{
    public class Indexing : ApplicationEventHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // initialization call validates indexer & searcher and then wires up the events
            LookService.Initialize(
                            this.Indexer_DocumentWriting,
                            new UmbracoHelper(UmbracoContext.Current));
        }

        private void Indexer_DocumentWriting(object sender, DocumentWritingEventArgs e, UmbracoHelper umbracoHelper, string indexerName)
        {
            IPublishedContent publishedContent = umbracoHelper.GetPublishedContent(e.NodeId);

            if (publishedContent != null)
            {
                this.EnsureUmbracoContext();

                var indexingContext = new IndexingContext(publishedContent, indexerName);

                LookService.Index(indexingContext, e.Document);
            }
        }

        /// <summary>
        /// HACK: this indexing event is on a thread outside of HttpContext, and context is required to get the url from IPublishedContent
        /// </summary>
        private void EnsureUmbracoContext()
        {
            var dummyHttpContext = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("", "", new StringWriter())));

            // commented out params as overload not available in Umbraco 7.2.3
            UmbracoContext.EnsureContext(
                                dummyHttpContext,
                                ApplicationContext.Current,
                                new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                                //UmbracoConfig.For.UmbracoSettings(),
                                //UrlProviderResolver.Current.Providers,
                                true,
                                false);
        }
    }
}
