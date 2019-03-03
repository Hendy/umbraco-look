using Examine;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Our.Umbraco.Look.BackOffice.Attributes
{
    public class ValidateSearcherAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var searcherName = actionContext.ActionArguments["searcherName"].ToString();

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];

            if (searcher == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unknown Searcher");
            }

            actionContext.RequestContext.RouteData.Values["searcher"] = searcher;
        }
    }
}
