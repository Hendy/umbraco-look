using Our.Umbraco.Look.Extensions;
using System;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Look.Models
{
    public class LookMatch
    {
        private Lazy<IPublishedContent> _node;

        /// <summary>
        /// The Umbraco (content, media or member) Id of the matched item
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Lazy evaluation of Item for IPublishedContent
        /// </summary>
        public IPublishedContent Node => this._node.Value;

        /// <summary>
        /// Enum flag to indicate if the IPublishedContent node is Content, Media or a Member
        /// </summary>
        public NodeType NodeType { get; }

        /// <summary>
        /// Highlight text (containing search text) extracted from from the full text
        /// </summary>
        public IHtmlString Highlight { get; }

        /// <summary>
        /// The full text (only returned if specified)
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Tag collection (only returned if specified)
        /// </summary>
        public LookTag[] Tags { get; }

        /// <summary>
        /// The custom date field
        /// </summary>
        public DateTime? Date { get; }

        /// <summary>
        /// The custom name field
        /// </summary>
        public string Name { get; }

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
        /// <param name="nodeType"></param>
        /// <param name="highlight"></param>
        /// <param name="text"></param>
        /// <param name="tags"></param>
        /// <param name="date"></param>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        internal LookMatch(
                    int id,
                    NodeType nodeType,
                    IHtmlString highlight,
                    string text,
                    LookTag[] tags,
                    DateTime? date,
                    string name,
                    Location location,
                    double? distance)
        {
            this.Id = id;
            this.NodeType = nodeType;
            this.Highlight = highlight;
            this.Text = text;
            this.Tags = tags;
            this.Date = date;
            this.Name = name;
            this.Location = location;
            this.Distance = distance;

            this._node = new Lazy<IPublishedContent>(() => {

                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                switch (this.NodeType)
                {
                    case NodeType.Content: return umbracoHelper.TypedContent(id);
                    case NodeType.Media: return umbracoHelper.TypedMedia(id);
                    case NodeType.Member: return umbracoHelper.SafeTypedMember(id);
                }

                throw new Exception("Unknown NodeType");
            });
        }
    }
}
