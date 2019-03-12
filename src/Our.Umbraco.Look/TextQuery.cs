namespace Our.Umbraco.Look
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
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var textQuery = obj as TextQuery;

            return textQuery != null
                && textQuery.SearchText == this.SearchText
                && textQuery.GetHighlight == this.GetHighlight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal TextQuery Clone()
        {
            return (TextQuery)this.MemberwiseClone();
        }
    }
}
