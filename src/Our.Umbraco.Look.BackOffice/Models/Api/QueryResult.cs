namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class QueryResult
    {
        /// <summary>
        /// array of matched Lucene documents (flattened from a LookResult)
        /// </summary>
        Match[] Matches { get; set; }

        ///// <summary>
        ///// Flag to use by lazy loader to know when to stop
        ///// </summary>
        //bool MoreItems { get; set; } = false;

        /// <summary>
        /// Total number of results matching the query supplied (ignores any skip/take values)
        /// </summary>
        public int TotalItemCount { get; set; } = -1;

    }
}
