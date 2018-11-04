namespace Our.Umbraco.Look.Models
{
    public class Facet
    {
        /// <summary>
        /// The tag
        /// </summary>
        public Tag Tag { get; internal set; }

        /// <summary>
        /// The total number of results expected should this tag be added to TagQuery.AllTags on the current query
        /// </summary>
        public int Count { get; internal set; }
    }
}
