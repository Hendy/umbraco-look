namespace Our.Umbraco.Look.Models
{
    public class Tag
    {
        /// <summary>
        /// Optional group for this tag
        /// </summary>
        public string Group { get; } = string.Empty;

        /// <summary>
        /// Name of this tag (the key)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor - create a tag in the default 'name-less' group
        /// </summary>
        /// <param name="name"></param>
        public Tag(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Constructor - create a tag in the specified group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        public Tag(string group, string name)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                group = string.Empty;
            }

            this.Group = group;
            this.Name = name;
        }
    }
}
