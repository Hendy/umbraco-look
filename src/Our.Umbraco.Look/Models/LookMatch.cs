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
        private Dictionary<string, string> _fieldSingleValues;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string[]> _fieldMultiValues;

        /// <summary>
        /// Lazy evaluation of Item for IPublishedContent
        /// </summary>
        public IPublishedContent Item => this._item.Value; // TODO: rename to Node

        /// <summary>
        /// 
        /// </summary>
        public new int DocId { get; set; } // get a& set, as replacing that in derived class

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
        public new IDictionary<string, string> Fields
        {
            get
            {
                return this._fieldSingleValues;
            }

            protected set
            {
                throw new NotSupportedException("Setting the Fields property is not supported");
            }
        }

        //public KeyValuePair<string, string> this[int index] => this._fieldSingleValues[index];

        //public new string this[string key]
        //{
        //    get
        //    {
        //        if (this._fieldSingleValues.TryGetValue(key, out string value))
        //        {
        //            return value;
        //        }

        //        return null;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="score"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="text"></param>
        /// <param name="highlight"></param>
        /// <param name="tags"></param>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <param name="fieldSingleValues"></param>
        /// <param name="fieldMultiValues"></param>
        /// <param name="publishedItemType"></param>
        /// <param name="umbracoHelper"></param>
        internal LookMatch(
                    int docId,
                    float score,
                    int id,
                    string name,
                    DateTime? date,
                    string text,
                    IHtmlString highlight,
                    LookTag[] tags,
                    Location location,
                    double? distance,
                    Dictionary<string, string> fieldSingleValues,
                    Dictionary<string, string[]> fieldMultiValues,
                    PublishedItemType publishedItemType,
                    UmbracoHelper umbracoHelper)
        {
            this.DocId = docId; // not in Examine 0.1.70, but in more recent versions
            this.Score = score;
            this.Id = id;
            this.Name = name;
            this.Date = date;
            this.Text = text;
            this.Highlight = highlight;
            this.Tags = tags;
            this.Location = location;
            this.Distance = distance;
            this._fieldSingleValues = fieldSingleValues;
            this._fieldMultiValues = fieldMultiValues;

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
            if (this._fieldMultiValues.TryGetValue(key, out string[] values))
            {
                return values;
            }

            if (this._fieldSingleValues.TryGetValue(key, out string value))
            {
                return new string[] { value };
            }

            return new string[] { };
        }
    }
}
