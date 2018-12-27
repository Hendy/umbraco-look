namespace Our.Umbraco.Look
{
    internal static class LookConstants
    {
        /// <summary>
        /// Unit tests should be the only ones where this is false
        /// </summary>
        internal static string HasNodeField => "Look_HasNode";

        /// <summary>
        /// Umbraco Id for the content, media or member being indexed (detached content will have an id = 0)
        /// </summary>
        internal static string NodeIdField => "Look_NodeId";

        /// <summary>
        /// Alternative to the Id field (detached nodes have keys, but not ids)
        /// </summary>
        internal static string NodeKeyField => "Look_NodeKey";

        /// <summary>
        /// Gets the field name to use for the node type (content || media || member)
        /// </summary>
        internal static string NodeTypeField => "Look_NodeType";

        /// <summary>
        /// Field to store the docType, mediaType, or MemberType alias
        /// </summary>
        internal static string NodeAliasField => "Look_NodeAlias";

        /// <summary>
        /// Gets the field name to use when setting a content node's culture info(s) (if available)
        /// </summary>
        internal static string CultureField => "Look_Culture";

        /// <summary>
        /// id to the content media or member item hosting this detached item
        /// </summary>
        internal static string HostIdField => "Look_HostId";

        /// <summary>
        /// 
        /// </summary>
        internal static string HasNameField => "Look_HasName";

        /// <summary>
        /// Gets the field name to use for the name
        /// </summary>
        internal static string NameField => "Look_Name";

        /// <summary>
        /// 
        /// </summary>
        internal static string HasDateField => "Look_HasDate";

        /// <summary>
        /// Gets the field name to use for the date - this fieldswill  stores the date as the number of seconds from the year 2000 (so it's a number that can be sorted)
        /// </summary>
        internal static string DateField => "Look_Date";

        /// <summary>
        /// 
        /// </summary>
        internal static string HasTextField => "Look_HasText";

        /// <summary>
        /// Gets the field name to use for the text - this field is expected to contain a sizeable amount of text
        /// </summary>
        internal static string TextField => "Look_Text";

        /// <summary>
        /// 
        /// </summary>
        internal static string HasTagsField => "Look_HasTags";

        /// <summary>
        /// Single field used to store 'group & name' for each tag, so can get all tags without first testing for field names prefixed with x / maintaining a memory state cache
        /// </summary>
        internal static string AllTagsField => "Look_AllTags";

        /// <summary>
        /// Gets the field name to use for the tags - this field will contain space delimited non-tokenizable strings
        /// </summary>
        internal static string TagsField => "Look_Tags_";

        /// <summary>
        /// Gets the field name to use to mark that the item being indexed has a location
        /// </summary>
        internal static string HasLocationField => "Look_HasLocation";

        /// <summary>
        /// Gets the field name to use for the location
        /// </summary>
        internal static string LocationField => "Look_Location";

        /// <summary>
        /// Gets the field prefix to use for location fields
        /// </summary>
        internal static string LocationTierFieldPrefix => "Look_Location_Tier_";

        /// <summary>
        /// Not stored in index, but used as a result field
        /// </summary>
        internal static string DistanceField => "Look_Distance";
    }
}
