namespace Our.Umbraco.Look.Models
{
    public enum SortOn
    {
        /// <summary>
        /// The Lucene result score (default)
        /// </summary>
        Score,

        /// <summary>
        /// A Custom Name field (alpha-numeric sorting)
        /// </summary>
        Name,

        /// <summary>
        /// A Custom Date field
        /// </summary>
        Date,

        /// <summary>
        /// Orders by distance (only if distance data avaialble, otherwise reverts back to score)
        /// </summary>
        Distance
    }
}
