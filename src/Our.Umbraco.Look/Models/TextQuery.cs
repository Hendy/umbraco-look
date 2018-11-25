namespace Our.Umbraco.Look.Models
{
    public class TextQuery
    {
        /// <summary>
        /// The text to search for
        /// </summary>
        public string SearchText { get; set; } = null;

        ///// <summary>
        ///// Optional fuzzyness factor
        ///// </summary>
        //public float Fuzzyness { get; set; } = 0;

        /// <summary>
        /// When true (and SearchText provided), a hightlight extract containing the search text will be returned
        /// </summary>
        public bool GetHighlight { get; set; } = false;

        /// <summary>
        /// Flag to indicate whether the source text field should be returned
        /// </summary>
        public bool GetText { get; set; } = false;

        ///// <summary>
        ///// 
        ///// </summary>
        //public TextQuery()
        //{
        //}

        //public TextQuery(string searchText)
        //{
        //	this.SearchText = searchText;
        //}	

        public override bool Equals(object obj)
        {
            var textQuery = obj as TextQuery;

            return textQuery != null
                && textQuery.SearchText == this.SearchText
                && textQuery.GetHighlight == this.GetHighlight
                && textQuery.GetText == this.GetText;
        }

        internal TextQuery Clone()
        {
            return (TextQuery)this.MemberwiseClone();
        }
    }
}
