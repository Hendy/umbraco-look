using Our.Umbraco.Look.Services;
using System.ComponentModel;
using Umbraco.Core;
using Umbraco.Web;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Hooks into all configured Exmaine Umbraco indexers (unless configured otherwise by the consumer)
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
            LookService.Initialize(new UmbracoHelper(UmbracoContext.Current));

            // if consumer hasn't (yet) set the examine indexers, then register them all
            if (!LookService.ExamineIndexersConfigured)
            {
                // register all
                LookService.SetExamineIndexers();
            }
        }
    }
}
