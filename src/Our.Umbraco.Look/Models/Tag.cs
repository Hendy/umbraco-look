namespace Our.Umbraco.Look.Models
{
    public class Tag
    {
        //public const string DEFAULT_GROUP = "default";

        /// <summary>
        /// Optional group for this tag (NOT YET USED)
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Name of this tag (the key)
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public Tag()
        //{
        //}

        /// <summary>
        /// Constructor - create a tag in the 'default' no group
        /// </summary>
        /// <param name="name"></param>
        public Tag(string name)
        {
            this.Name = name;
        }

        ///// <summary>
        ///// Constructor - create a tag in the specified group
        ///// </summary>
        ///// <param name="group"></param>
        ///// <param name="name"></param>
        //public Tag(string group, string name)
        //{
        //    this.Group = group;
        //    this.Name = name;
        //}


        //public Tag(string name, string group = Tag.DEFAULT_GROUP)
        //{

        //}
    }
}
