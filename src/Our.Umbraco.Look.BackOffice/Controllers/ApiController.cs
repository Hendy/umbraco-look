using Examine;
using Our.Umbraco.Look;
using Our.Umbraco.Look.BackOffice.Models.Api;
using Our.Umbraco.Look.BackOffice.Services;
using System.Linq;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.AzureLogger.Core.Controllers
{
    [PluginController("Look")]
    public class ApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public IHttpActionResult GetViewDataForSearcher([FromUri]string searcherName)
        {
            var viewData = new SearcherViewData();

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];
            if (searcher == null) { return this.BadRequest("Unknown Searcher"); }

            viewData.SearcherName = searcher.Name;
            viewData.SearcherDescription = searcher.Description;
            viewData.SearcherType = searcher is LookSearcher ? "Look" : "Examine";
            viewData.Icon = TreeService.GetSearcherIcon(searcher);

            // number of documents in index
            // indexers operational

            return this.Ok(viewData);
        }

        /// <summary>
        /// Get the view model for the top level 'Tags' tree node (for a searcher)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetViewDataForTags([FromUri]string searcherName)
        {
            var viewData = new TagsViewData();

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];
            if (searcher == null) { return this.BadRequest("Unknown Searcher"); }

            // The tags node renders all tag groups
            viewData.TagGroups = QueryService.GetTagGroups(searcherName);

            return this.Ok(viewData);
        }

        /// <summary>
        /// Get the view model for a specific 'TagGroup' tree node (for a searcher)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetViewDataForTagGroup([FromUri]string searcherName, [FromUri]string tagGroup)
        {
            var viewData = new TagGroupViewData();

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];
            if (searcher == null) { return this.BadRequest("Unknown Searcher"); }

            viewData.Tags = QueryService.GetTags(searcherName, tagGroup);

            return this.Ok(viewData);
        }

        /// <summary>
        /// Get the view model for a specific 'Tag' tree node (for a searcher)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetViewDataForTag([FromUri]string searcherName, [FromUri]string tagGroup, [FromUri]string tagName)
        {
            var viewData = new TagViewData();

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];
            if (searcher == null) { return this.BadRequest("Unknown Searcher"); }


            return this.Ok(viewData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="tagGroup"></param>
        /// <param name="tagName"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetMatches(
                    [FromUri]string searcherName, 
                    [FromUri]string tagGroup, 
                    [FromUri]string tagName,
                    [FromUri]string sort,
                    [FromUri]string skip,
                    [FromUri]string take)
        {
            var matchesResult = new MatchesResult();

            var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];
            if (searcher == null) { return this.BadRequest("Unknown Searcher"); }


            matchesResult.Matches = new MatchesResult.Match[] { new MatchesResult.Match() {  } };




            return this.Ok(matchesResult);
        }

    }
}
