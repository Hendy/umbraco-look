using Examine;
using Our.Umbraco.Look.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Models
{
    public class LookMatch : SearchResult
    {
        private Lazy<IPublishedContent> _item;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string[]> _fieldValues;

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
        /// 
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="score"></param>
        /// <param name="fieldValues"></param>
        /// <param name="id"></param>
        /// <param name="publishedItemType"></param>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="text"></param>
        /// <param name="highlight"></param>
        /// <param name="tags"></param>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <param name="umbracoHelper"></param>
        internal LookMatch(
                    int docId,
                    float score,
                    Dictionary<string, string[]> fieldValues,
                    int id,
                    PublishedItemType publishedItemType,
                    string name,
                    DateTime? date,
                    string text,
                    IHtmlString highlight,
                    LookTag[] tags,
                    Location location,
                    double? distance,
                    UmbracoHelper umbracoHelper)
        {
            //this.DocId = docId; // added in a later version of Examine
            this.Score = score;
            this._fieldValues = fieldValues;
            // populate the inherited field collection (note multi-field values take the first - TODO: may need to remove multi values to be 100% compatable with how Examine does it)
            this.Fields = this._fieldValues.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
            this.Id = id;
            this.Name = name;
            this.Date = date;
            this.Text = text;
            this.Highlight = highlight;
            this.Tags = tags;
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

        /// <summary>
        /// replace the inherited behaviour so we can include multi-field values
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IEnumerable<string> GetValues(string key)
        {
            return this._fieldValues[key];
        }
    }
}
