namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Helper to parse null dictionary item
        /// </summary>
        /// <param name="indexerName"></param>
        /// <returns></returns>
        internal static IndexerConfiguration GetIndexerConfiguration(string indexerName)
        {
            if (LookConfiguration.IndexerConfiguration.ContainsKey(indexerName))
            {
                return LookConfiguration.IndexerConfiguration[indexerName];
            }

            return new IndexerConfiguration();
        }
    }
}
