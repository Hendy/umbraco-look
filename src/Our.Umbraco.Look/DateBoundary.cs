namespace Our.Umbraco.Look
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
        /// Before date inclusive, after date exclusive
        /// </summary>
        BeforeInclusiveAfterExclusive,

        /// <summary>
        /// Before date exclusive, after date inclusive
        /// </summary>
        BeforeExclusiveAfterInclusive,
    }
}
