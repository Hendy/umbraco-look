namespace Our.Umbraco.Look.Models
{
    public class TextQuery
    {
        /// <summary>
        /// The text to search for
        /// </summary>
        public string SearchText { get; set; } = null;

        /// <summary>
        /// When true (and SearchText provided), a hightlight extract containing the search text will be returned
        /// </summary>
        public bool GetHighlight { get; set; } = false;

        /// <summary>
        /// Create a new TextQuery search criteria
        /// </summary>
        /// <param name="searchText">The text to search for - can include wildcards</param>
        /// <param name="getHighlight">Set to true to return highlight fragment per result</param>
        public TextQuery(string searchText = null, bool getHighlight = false)
        {
            this.SearchText = searchText;
            this.GetHighlight = getHighlight;
        }

        public override bool Equals(object obj)
        {
            var textQuery = obj as TextQuery;

            return textQuery != null
                && textQuery.SearchText == this.SearchText
                && textQuery.GetHighlight == this.GetHighlight;
        }

        internal TextQuery Clone()
        {
            return (TextQuery)this.MemberwiseClone();
        }
    }
}
