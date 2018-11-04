namespace Our.Umbraco.Look
{
    internal static class LookConstants
    {
        /// <summary>
        /// Gets the field name to use for the text - this field is expected to contain a sizeable amount of text
        /// </summary>
        internal static string TextField => "Our.Umbraco.Look_Text";

        /// <summary>
        /// Gets the field name to use for the tags - this field will contain space delimited non-tokenizable strings
        /// </summary>
        internal static string TagsField => "Our.Umbraco.Look_Tags_";

        /// <summary>
        /// Gets the field name to use for the date - this fieldswill  stores the date as the number of seconds from the year 2000 (so it's a number that can be sorted)
        /// </summary>
        internal static string DateField => "Our.Umbraco.Look_Date";

        /// <summary>
        /// Gets the field name to use for the name
        /// </summary>
        internal static string NameField => "Our.Umbraco.Look_Name";

        /// <summary>
        /// Gets the field name to use for the location
        /// </summary>
        internal static string LocationField => "Our.Umbraco.Look_Location";

        /// <summary>
        /// not stored in index, but used as a result field
        /// </summary>
        internal static string DistanceField => "Our.Umbraco.Look_Distance";
    }
}
