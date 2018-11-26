namespace Our.Umbraco.Look
{
    internal static class LookConstants
    {
        /// <summary>
        /// Stored in custom field
        /// </summary>
        internal static string NodeTypeField => "Look_NodeType";

        ///// <summary>
        ///// Stored in a custom field
        ///// </summary>
        //internal static string NodeAliasField => "Look_NodeAlias";

        /// <summary>
        /// Gets the field name to use for the name
        /// </summary>
        internal static string NameField => "Look_Name";

        /// <summary>
        /// Gets the field name to use for the date - this fieldswill  stores the date as the number of seconds from the year 2000 (so it's a number that can be sorted)
        /// </summary>
        internal static string DateField => "Look_Date";

        /// <summary>
        /// Gets the field name to use for the text - this field is expected to contain a sizeable amount of text
        /// </summary>
        internal static string TextField => "Look_Text";

        /// <summary>
        /// Single field used to store 'group & name' for each tag, so can get all tags without first testing for field names prefixed with x / maintaining a memory state cache
        /// </summary>
        internal static string AllTagsField => "Look_AllTags";

        /// <summary>
        /// Gets the field name to use for the tags - this field will contain space delimited non-tokenizable strings
        /// </summary>
        internal static string TagsField => "Look_Tags_";

        /// <summary>
        /// Gets the field name to use for the location
        /// </summary>
        internal static string LocationField => "Look_Location";

        /// <summary>
        /// 
        /// </summary>
        internal static string LocationTierFieldPrefix => "Look_Location_Tier_";

        /// <summary>
        /// not stored in index, but used as a result field
        /// </summary>
        internal static string DistanceField => "Look_Distance";
    }
}
