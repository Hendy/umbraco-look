using Examine;
using Our.Umbraco.Look.BackOffice.Models.Api;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.AzureLogger.Core.Controllers
{
    [PluginController("Look")]
    public class ApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public IHttpActionResult GetSearcherDetails([FromUri]string searcherName)
        {
            var response = new SearcherDetailsResponse();

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];

            if (searcher == null)
            {
                return this.BadRequest("Unknown Searcher");
            }

            return this.Ok(response);
        }
    
    }
}
