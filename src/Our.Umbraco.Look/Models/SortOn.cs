namespace Our.Umbraco.Look.Models
{
    public enum SortOn
    {
        /// <summary>
        /// The Lucene result score (default)
        /// </summary>
        Score = 0,

        /// <summary>
        /// A Custom Name field (alpha-numeric sorting)
        /// </summary>
        Name = 1,

        /// <summary>
        /// A Custom Date field - Old to New
        /// </summary>
        DateAscending = 2,

        /// <summary>
        /// A Custom Date field - New to Old
        /// </summary>
        DateDescending = 3,

        /// <summary>
        /// Orders by distance (only if distance data avaialble, otherwise reverts back to score)
        /// </summary>
        Distance = 4
    }
}
