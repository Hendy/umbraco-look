using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.AzureLogger.Core.Controllers
{
    [PluginController("Look")]
    public class ApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object GetSearcherDetails([FromUri]string searcherName)
        {



            return null;
        }
    
    }
}
