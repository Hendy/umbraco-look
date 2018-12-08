using Examine.LuceneEngine;
using Our.Umbraco.Look.Extensions;
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

            if (publishedContent == null) // attempt to get as media
            {
                publishedContent = umbracoHelper.TypedMedia(e.NodeId);

                if (publishedContent == null) // attempt to get as member
                {
                    publishedContent = umbracoHelper.SafeTypedMember(e.NodeId);
                }
            }
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
