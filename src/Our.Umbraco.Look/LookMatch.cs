using Examine;
using Newtonsoft.Json;
using Our.Umbraco.Look.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look
{
    public class LookMatch : SearchResult
    {
        /// <summary>
        /// This is set if the Item returned is detached content (otherwise will be null)
        /// </summary>
        private Lazy<IPublishedContent> _hostItem;

        /// <summary>
        /// This is the content, media, member or detached item
        /// </summary>
        private Lazy<IPublishedContent> _item;

        /// <summary>
        /// The Examine searcher used
        /// </summary>
        [JsonIgnore]
        public string SearcherName { get; }

        /// <summary>
        /// Lazy evaluation of the host item (if the item is detached) otherwize this will be null
        /// </summary>
        [JsonIgnore]
        public IPublishedContent HostItem => this._hostItem.Value;

        /// <summary>
        /// Lazy evaluation of Item for IPublishedContent of the content, media, member or detached item
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Item => this._item.Value;

        /// <summary>
        /// Culture in Umbraco associate with this node
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Flag to indicate whether this result is a detached item
        /// </summary>
        public bool IsDetached { get; }

        /// <summary>
        /// Guid key of the Content, media, member or detached item
        /// </summary>
        public Guid Key { get; }

        /// <summary>
        /// The custom name field
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The custom date field
        /// </summary>
        public DateTime? Date { get; }

        /// <summary>
        /// The full text (only returned if specified)
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Highlight text (containing search text) extracted from from the full text
        /// </summary>
        public IHtmlString Highlight { get; }

        /// <summary>
        /// Tag collection (only returned if specified)
        /// </summary>
        public LookTag[] Tags { get; }

        /// <summary>
        /// The custom location (lat|lng) field
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Temp field for calculated results
        /// </summary>
        public double? Distance { get; }

        /// <summary>
        /// The contextual type: content, media or member (a detached item belongs to one of these)
        /// </summary>
        public PublishedItemType PublishedItemType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcherName"></param>
        /// <param name="docId"></param>
        /// <param name="score"></param>
        /// <param name="hostId">host (if item is detached)</param>
        /// <param name="itemId">passed in so we don't have to get infalte IPublishedContent from itemGuid to get the int (required for the base SearchResult)</param>
        /// <param name="itemGuid">expected to be null in unit tests (as outside of Umbraco context)</param>
        /// <param name="cultureInfo"></param>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="text"></param>
        /// <param name="highlight"></param>
        /// <param name="tags"></param>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <param name="publishedItemType"></param>
        /// <param name="umbracoHelper"></param>
        internal LookMatch(
                    string searcherName,
                    int docId,
                    float score,
                    int? hostId,
                    int itemId, 
                    Guid? itemGuid,
                    CultureInfo cultureInfo,
                    string name,
                    DateTime? date,
                    string text,
                    IHtmlString highlight,
                    LookTag[] tags,
                    Location location,
                    double? distance,
                    PublishedItemType publishedItemType,
                    UmbracoHelper umbracoHelper)
        {
            this.SearcherName = searcherName;
            //this.DocId = docId; // not in Examine 0.1.70, but in more recent versions
            this.Score = score;
            this.Id = itemId;
            this.Key = itemGuid ?? Guid.Empty;
            this.CultureInfo = cultureInfo;
            this.IsDetached = hostId.HasValue;
            this.Name = name;
            this.Date = date;
            this.Text = text;
            this.Highlight = highlight;
            this.Tags = tags;
            this.Location = location;
            this.Distance = distance;
            this.PublishedItemType = publishedItemType;

            this._hostItem = new Lazy<IPublishedContent>(() =>
            {
                if (umbracoHelper != null && this.IsDetached)
                {
                    switch (publishedItemType)
                    {
                        case PublishedItemType.Content: return umbracoHelper.TypedContent(hostId.Value); 
                        case PublishedItemType.Media: return umbracoHelper.TypedMedia(hostId.Value);
                        case PublishedItemType.Member: return umbracoHelper.SafeTypedMember(hostId.Value);
                    }
                }

                return null;
            });

            this._item = new Lazy<IPublishedContent>(() => 
            {
                if (umbracoHelper != null) // will be null for unit tests (as not initialized via Umbraco startup)
                {
                    if (this.HostItem == null) // not detached
                    {
                        switch (publishedItemType)
                        {
                            case PublishedItemType.Content: return umbracoHelper.TypedContent(itemId);
                            case PublishedItemType.Media: return umbracoHelper.TypedMedia(itemId);
                            case PublishedItemType.Member: return umbracoHelper.SafeTypedMember(itemId);
                        }
                    }
                    else // get the host item and then all of it's detached items to find by key
                    {                        
                        return this.HostItem.GetFlatDetachedDescendants().SingleOrDefault(x => x.GetKey() == itemGuid.Value);
                    }
                }

                return null;
            });
        }
    }
}
