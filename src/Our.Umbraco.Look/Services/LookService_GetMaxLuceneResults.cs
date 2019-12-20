namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get the default max lucene results value
        /// </summary>
        /// <returns></returns>
        internal static int GetMaxLuceneResults()
        {
            return LookService.Instance._maxLuceneResults;
        }
    }
}