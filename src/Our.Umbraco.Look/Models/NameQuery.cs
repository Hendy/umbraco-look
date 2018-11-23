namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Search query criteria for use with the custom name field
    /// </summary>
    public class NameQuery
    {
        /// <summary>
        /// (Optional) set the string which the name must begin with
        /// </summary>
        public string StartsWith { get; set; }

        /// <summary>
        /// (Optional) set the string which the name must end with
        /// </summary>
        public string EndsWith { get; set; }

        /// <summary>
        /// (Optional) set the string which must be present in the name
        /// </summary>
        public string Contains { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool CaseSensitive { get; set; } = true;
    }
}
