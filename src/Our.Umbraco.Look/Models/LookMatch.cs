using System;
using System.Web;

namespace Our.Umbraco.Look.Models
{
    public class LookMatch
    {
        /// <summary>
        /// The Umbraco (content, media or member) Id of the matched item
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Highlight text (containing search text) extracted from from the full text
        /// </summary>
        public IHtmlString Highlight { get; internal set; }

        /// <summary>
        /// The full text (only returned if specified)
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// Tag collection (only returned if specified)
        /// </summary>
        public string[] Tags { get; internal set; }

        /// <summary>
        /// The custom date field
        /// </summary>
        public DateTime? Date { get; internal set; }

        /// <summary>
        /// The custom name field
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The custom location (lat|lng) field
        /// </summary>
        public Location Location { get; internal set; }

        /// <summary>
        /// Temp field for calculated results
        /// </summary>
        public double? Distance { get; internal set; }

        /// <summary>
        /// The Lucene score for this match
        /// </summary>
        public float Score { get; internal set; }
    }
}
