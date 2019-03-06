using Our.Umbraco.Look.Services;
using System.ComponentModel;
using Umbraco.Core;
using Umbraco.Web;

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
            LookService.Initialize(new UmbracoHelper(UmbracoContext.Current));

            // if consumer hasn't (yet) set the examine indexers to use then register them all
            if (!LookService.ExamineIndexersConfigured)
            {
                LookService.SetExamineIndexers(); // wire-up all
            }
        }
    }
}
