namespace Our.Umbraco.Look.Models
{
    public class TextQuery
    {
        /// <summary>
        /// The text to search for
        /// </summary>
        public string SearchText { get; set; } = null;

        /// <summary>
        /// Optional fuzzyness factor
        /// </summary>
        public float Fuzzyness { get; set; } = 0;

        /// <summary>
        /// The max number of fragments to show, 0 = highlighting disabled
        /// </summary>
        public int HighlightFragments { get; set; } = 0;

        /// <summary>
        /// Text rendered between any highlight fragments
        /// </summary>
        public string HighlightSeparator { get; set; } = "...";

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
    }
}
