using Examine.Providers;
using Our.Umbraco.Look;
using Our.Umbraco.Look.BackOffice.Attributes;
using Our.Umbraco.Look.BackOffice.Models.Api;
using Our.Umbraco.Look.BackOffice.Services;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.AzureLogger.Core.Controllers
{
    [PluginController("Look")]
    public class ApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        [ValidateSearcher] // if searcher name valid, then sets routeData value 'searcher'
        public IHttpActionResult GetViewDataForSearcher([FromUri]string searcherName)
        {
            var viewData = new SearcherViewData();

            var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];
            //var searcher = ExamineManager.Instance.SearchProviderCollection[searcherName];

            viewData.SearcherName = searcher.Name;
            viewData.SearcherDescription = searcher.Description;
            viewData.SearcherType = searcher is LookSearcher ? "Look" : "Examine";
            viewData.Icon = IconService.GetSearcherIcon(searcher);

            //viewData.LookIndexingEnabled = viewData.SearcherDescription == "Look" || 
            //viewData.NameIndexerEnabled = 
            // number of documents in index
            // indexers operational
            //viewData.LookIndexingEnabled = 

            return this.Ok(viewData);
        }

        /// <summary>
        /// Get the view model for the top level 'Tags' tree node (for a searcher)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForNodes([FromUri]string searcherName)
        {
            var viewData = new NodesViewData();

            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            return this.Ok(viewData);
        }

        // TODO: GetViewDataForNodeType
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForNodeType([FromUri]string searcherName, [FromUri]PublishedItemType nodeType)
        {
            var viewData = new NodeTypeViewData();

            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            //viewData.NodeType = nodeType;
            viewData.Name = nodeType == PublishedItemType.Content ? "Content"
                            : nodeType == PublishedItemType.Media ? "Media"
                            : nodeType == PublishedItemType.Member ? "Members"
                            : null;

            viewData.Icon = IconService.GetNodeTypeIcon(nodeType);

            return this.Ok(viewData);
        }


        /// <summary>
        /// Get the view model for the top level 'Tags' tree node (for a searcher)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForTags([FromUri]string searcherName)
        {
            var viewData = new TagsViewData();

            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

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
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForTagGroup([FromUri]string searcherName, [FromUri]string tagGroup)
        {
            var viewData = new TagGroupViewData();

            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            viewData.Tags = QueryService.GetTags(searcherName, tagGroup);

            return this.Ok(viewData);
        }

        /// <summary>
        /// Get the view model for a specific 'Tag' tree node (for a searcher)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForTag([FromUri]string searcherName, [FromUri]string tagGroup, [FromUri]string tagName)
        {
            var viewData = new TagViewData();

            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            return this.Ok(viewData);
        }

        /// <summary>
        /// Get the view model for the 'Locations' tree node (for a searcher)
        /// </summary>
        /// <param name="searcherName"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForLocations([FromUri]string searcherName)
        {
            var viewData = new LocationsViewData();

            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            return this.Ok(viewData);
        }

        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetMatches(
            [FromUri]string searcherName,
            [FromUri]string sort,
            [FromUri]int skip,
            [FromUri]int take)
        {
            var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            return this.Ok(QueryService.GetMatches(searcherName, sort, skip, take));
        }

        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetNodeTypeMatches(
            [FromUri]string searcherName,
            [FromUri]PublishedItemType nodeType,
            [FromUri]string sort,
            [FromUri]int skip,
            [FromUri]int take)
        {
            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            return this.Ok(QueryService.GetNodeTypeMatches(searcherName, nodeType, sort, skip, take));
        }

        /// <summary>
        /// Get matches based on tags
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="tagGroup"></param>
        /// <param name="tagName"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetTagMatches(
                    [FromUri]string searcherName, 
                    [FromUri]string tagGroup, 
                    [FromUri]string tagName,
                    [FromUri]string sort,
                    [FromUri]int skip,
                    [FromUri]int take)
        {
            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            return this.Ok(QueryService.GetTagMatches(searcherName, tagGroup, tagName, sort, skip, take));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetLocationMatches(
            [FromUri]string searcherName,
            [FromUri]string sort,
            [FromUri]int skip,
            [FromUri]int take)
        {
            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            return this.Ok(QueryService.GetLocationMatches(searcherName, sort, skip, take));
        }

        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetConfigurationData([FromUri]string searcherName)
        {
            //var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"];

            // TODO:

            var configurationData = new ConfigurationData();

            return this.Ok(configurationData);
        }
    }
}
