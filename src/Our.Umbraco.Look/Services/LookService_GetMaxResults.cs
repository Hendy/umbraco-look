namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get the default max number of results to return value
        /// </summary>
        /// <returns></returns>
        internal static int GetMaxResults()
        {
            return LookService.Instance._maxResults;
        }
    }
}