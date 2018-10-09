using Examine;
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
using UmbracoExamine;

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
            LookService.Initialize(
                            this.Indexer_GatheringNodeData,
                            new UmbracoHelper(UmbracoContext.Current));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="umbracoHelper"></param>
        private void Indexer_GatheringNodeData(object sender, IndexingNodeDataEventArgs e, UmbracoHelper umbracoHelper)
        {
            IPublishedContent publishedContent = null;

            switch (e.IndexType)
            {
                case IndexTypes.Content: publishedContent = umbracoHelper.TypedContent(e.NodeId); break;
                case IndexTypes.Media: publishedContent = umbracoHelper.TypedMedia(e.NodeId); break;
                case IndexTypes.Member: publishedContent = umbracoHelper.TypedMember(e.NodeId); break;
            }

            if (publishedContent != null)
            {
                this.EnsureUmbracoContext();

                LookIndexService.Index(publishedContent, e);
            }
        }

        /// <summary>
        /// HACK: this indexing event is on a thread outside of HttpContext, and context is required to get the url from IPubishedContent
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
