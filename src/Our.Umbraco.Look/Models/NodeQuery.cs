namespace Our.Umbraco.Look.Models
{
    public class NodeQuery
    {
        /// <summary>
        /// Collection of Node type aliases that a results can be any one of (when empty = has no effect)
        /// </summary>
        public string[] TypeAliases { get; set; } = null;

        /// <summary>
        /// Any umbraco ids that should be exlcuded from the results (performed as part of the Lucene query so as to get an accurate result count)
        /// </summary>
        public int[] ExcludeIds { get; set; } = null;

        ///// <summary>
        ///// Constructor
        ///// </summary>
        //public NodeQuery()
        //{
        //}

        //public NodeQuery(string typeAlias)
        //{
        //    this.TypeAliases = new string[] { typeAlias };
        //}

        //public NodeQuery(int notId)
        //{
        //    this.ExcludeIds = new int[] { notId };
        //}

        //public NodeQuery(string typeAlias, int notId)
        //{
        //    this.TypeAliases = new string[] { typeAlias };
        //    this.ExcludeIds = new int[] { notId };
        //}
    }
}
