namespace Our.Umbraco.Look
{
    public class Facet
    {
        /// <summary>
        /// The tags that would be added into the TagQuery.All clause
        /// </summary>
        public LookTag[] Tags { get; internal set; }

        //public Distance Distance { get; internal set; }

        /// <summary>
        /// The total number of results expected should this facet be applied to the curent query
        /// </summary>
        public int Count { get; internal set; }

        ///// <summary>
        ///// helper to see if the tags collection for this facet is a single item, matching that supplied
        ///// </summary>
        ///// <param name="lookTag"></param>
        //internal bool Equals(LookTag lookTag)
        //{
        //    return lookTag != null && this.Tags.Length == 1 && this.Tags[0].Equals(lookTag);
        //}
    }
}
