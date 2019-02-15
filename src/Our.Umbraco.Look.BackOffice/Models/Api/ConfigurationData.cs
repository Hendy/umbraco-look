namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    public class ConfigurationData
    {
        /// <summary>
        /// when true indicates that a name indexer has been registered
        /// </summary>
        public bool NameIndexerSet { get; set; }

        /// <summary>
        /// when true indicates that a date indexer has been registered
        /// </summary>
        public bool DateIndexerSet { get; set; }

        /// <summary>
        /// when true indicates that a text indexer has been registered
        /// </summary>
        public bool TextIndexerSet { get; set; }

        /// <summary>
        /// when true indicates that a tag indexer has been registered
        /// </summary>
        public bool TagIndexerSer { get; set; }

        /// <summary>
        /// when true indicates that a location indexer has been registered
        /// </summary>
        public bool LocationIndexerSer { get; set; }
    }
}
