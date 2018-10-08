using System;
using System.Web;

namespace Our.Umbraco.Look.Models
{
    public class LookResult
    {
        /// <summary>
        /// Required - represents the Umbraco (content, media or member) Id of the matched item
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Optional - (as requires additional lucene queries for each result)
        /// </summary>
        //public IHtmlString Highlight { get; internal set; }

        public string Text { get; internal set; }

        public string[] Tags { get; internal set; }

        public DateTime? Date { get; internal set; }

        public string Name { get; internal set; }

        public Location Location { get; internal set; }

        public double? Distance { get; internal set; }

        public float Score { get; internal set; }
    }
}
