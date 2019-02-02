using Examine;
using Our.Umbraco.Look;
using Our.Umbraco.Look.BackOffice.Models.Api;
using Our.Umbraco.Look.BackOffice.Services;
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

            response.SearcherName = searcher.Name;
            response.SearcherDescription = searcher.Description;
            response.SearcherType = searcher is LookSearcher ? "Look" : "Examine";
            response.Icon = LookTreeService.GetSearcherIcon(searcher);

            // number of documents in index
            // indexers operational

            return this.Ok(response);
        }
    
    }
}
