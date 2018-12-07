using Examine;
using Our.Umbraco.Look.Extensions;
using System;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Models
{
    public class LookMatch : SearchResult
    {
        private Lazy<IPublishedContent> _item;

        /// <summary>
        /// Lazy evaluation of Item for IPublishedContent
        /// </summary>
        public IPublishedContent Item => this._item.Value;

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
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="umbracoHelper"></param>
        /// <param name="publishedItemType"></param>
        /// <param name="highlight"></param>
        /// <param name="text"></param>
        /// <param name="tags"></param>
        /// <param name="date"></param>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        internal LookMatch(
                    UmbracoHelper umbracoHelper,
                    int id,
                    PublishedItemType publishedItemType,
                    IHtmlString highlight,
                    string text,
                    LookTag[] tags,
                    DateTime? date,
                    string name,
                    Location location,
                    double? distance)
        {
            this.Id = id;
            this.Highlight = highlight;
            this.Text = text;
            this.Tags = tags;
            this.Date = date;
            this.Name = name;
            this.Location = location;
            this.Distance = distance;

            this._item = new Lazy<IPublishedContent>(() => {

                if (umbracoHelper != null) // will be null for unit tests (as not initialized via Umbraco startup)
                {
                    switch (publishedItemType)
                    {
                        case PublishedItemType.Content: return umbracoHelper.TypedContent(id);
                        case PublishedItemType.Media: return umbracoHelper.TypedMedia(id);
                        case PublishedItemType.Member: return umbracoHelper.SafeTypedMember(id);
                    }
                }

                return null;
            });
        }
    }
}
