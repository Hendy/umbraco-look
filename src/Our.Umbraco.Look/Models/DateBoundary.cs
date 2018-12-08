namespace Our.Umbraco.Look.Models
{
    public enum DateBoundary
    {
        /// <summary>
        /// Both the dates will be inclusive
        /// </summary>
        Inclusive,

        /// <summary>
        /// Both the dates will be exclusive
        /// </summary>
        Exclusive,

        /// <summary>
        /// 
        /// </summary>
        AfterInclusiveBeforeExclusive,

        /// <summary>
        /// 
        /// </summary>
        AfterExclusiveBeforeInclusive,
    }
}
