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
        #region ViewDataService

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

            viewData.TagCounts = new LookQuery(searcherName) { TagQuery = new TagQuery() { FacetOn = new TagFacetQuery(tagGroup) } }
                                .Search()
                                .Facets
                                .Select(x => new TagGroupViewData.TagCount()
                                {
                                    Name = x.Tags.Single().Name, // query such that only single collection results expected
                                    Count = x.Count
                                })
                                .OrderBy(x => x.Name)
                                .ToArray();

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

        #endregion

        #region QueryService




        #endregion
    }
}
