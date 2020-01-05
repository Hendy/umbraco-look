namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Set the default MaxResults value (defaults to 5000 if not specified)
        /// </summary>
        /// <param name="maxResults"></param>
        internal static void SetMaxResults(int maxResults)
        {
            if (maxResults > 0) // ensure it's a valid value
            {
                LookService.Instance._maxResults = maxResults;
            }
        }
    }
}
