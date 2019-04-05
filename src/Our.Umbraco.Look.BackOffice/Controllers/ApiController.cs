﻿using Examine;
using Examine.Providers;
using Our.Umbraco.Look;
using Our.Umbraco.Look.BackOffice.Attributes;
using Our.Umbraco.Look.BackOffice.Models.Api;
using Our.Umbraco.Look.BackOffice.Services;
using System.Linq;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.AzureLogger.Core.Controllers
{
    [PluginController("Look")]
    public class ApiController : UmbracoAuthorizedApiController
    {
        #region Get ViewData

        [HttpGet]
        [ValidateSearcher] // if searcher name valid, then sets routeData value 'searcher'
        public IHttpActionResult GetViewDataForSearcher([FromUri]string searcherName)
        {
            var viewData = new SearcherViewData();

            var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"]; //ExamineManager.Instance.SearchProviderCollection[searcherName];

            viewData.SearcherName = searcher.Name;
            viewData.SearcherDescription = searcher.Description;
            viewData.SearcherType = searcher is LookSearcher ? "Look" : "Examine";
            viewData.Icon = IconService.GetSearcherIcon(searcher);
            viewData.LookIndexingEnabled = searcher is LookSearcher || LookConfiguration.ExamineIndexers.Contains(searcher.Name.TrimEnd("Searcher") + "Indexer");
            viewData.NameIndexerIsSet = viewData.LookIndexingEnabled && LookConfiguration.NameIndexerIsSet;
            viewData.DateIndexerIsSet = viewData.LookIndexingEnabled && LookConfiguration.DateIndexerIsSet;
            viewData.TextIndexerIsSet = viewData.LookIndexingEnabled && LookConfiguration.TextIndexerIsSet;
            viewData.TagIndexerIsSet = viewData.LookIndexingEnabled && LookConfiguration.TagIndexerIsSet;
            viewData.LocationIndexerIsSet = viewData.LookIndexingEnabled && LookConfiguration.LocationIndexerIsSet;

            // TODO: 
            //number of documents in index
            //fields in index
            //text analyzer
            //lucene index folder

            return this.Ok(viewData);
        }

        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForRebuild([FromUri]string searcherName)
        {
            var viewData = new RebuildViewData();

            var searcher = (BaseSearchProvider)this.RequestContext.RouteData.Values["searcher"]; //ExamineManager.Instance.SearchProviderCollection[searcherName];

            // is there a single index that matches this searcher - if 1 to 1 then re-indexing can happen, else warning to view
            var indexerName = searcher.Name.TrimEnd("Searcher") + "Indexer";

            var indexer = ExamineManager.Instance.IndexProviderCollection.SingleOrDefault(x => x.Name == indexerName);

            if (indexer != null)
            {
                viewData.ValidIndexer = true;
                viewData.IndexerName = indexer.Name;
            }
            
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

            return this.Ok(viewData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForNodeType([FromUri]string searcherName, [FromUri]PublishedItemType nodeType)
        {
            var viewData = new NodeTypeViewData();

            viewData.Name = nodeType == PublishedItemType.Content ? "Content"
                            : nodeType == PublishedItemType.Media ? "Media"
                            : nodeType == PublishedItemType.Member ? "Members"
                            : null;

            viewData.Icon = IconService.GetNodeTypeIcon(nodeType);

            return this.Ok(viewData);
        }

        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetViewDataForDetached([FromUri]string searcherName, [FromUri]PublishedItemType nodeType)
        {
            var viewData = new DetachedViewData();

            viewData.Name = nodeType == PublishedItemType.Content ? "Detached Content"
                            : nodeType == PublishedItemType.Media ? "Detached Media"
                            : nodeType == PublishedItemType.Member ? "Detached Member"
                            : null;

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

            return this.Ok(viewData);
        }

        #endregion

        #region Get Matches

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
            return this.Ok(QueryService.GetNodeTypeMatches(searcherName, nodeType, sort, skip, take));
        }

        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetDetachedeMatches(
            [FromUri]string searcherName,
            [FromUri]PublishedItemType nodeType,
            [FromUri]string sort,
            [FromUri]int skip,
            [FromUri]int take)
        {
            return this.Ok(QueryService.GetDetachedMatches(searcherName, nodeType, sort, skip, take));
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
            return this.Ok(QueryService.GetLocationMatches(searcherName, sort, skip, take));
        }

        #endregion

        /// <summary>
        /// Rebuild the Emamine index
        /// </summary>
        /// <param name="indexerName"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult RebuildIndex([FromUri]string indexerName)
        {
            var indexer = ExamineManager.Instance.IndexProviderCollection.SingleOrDefault(x => x.Name == indexerName);

            if (indexer != null)
            {
                // NOTE: not async
                indexer.RebuildIndex();

                return this.Ok();
            }

            return this.BadRequest("Unknown Indexer");
        }
           
        [HttpGet]
        [ValidateSearcher]
        public IHttpActionResult GetConfigurationData([FromUri]string searcherName)
        {
            // TODO:

            var configurationData = new ConfigurationData();

            return this.Ok(configurationData);
        }
    }
}
