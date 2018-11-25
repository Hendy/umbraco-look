namespace Our.Umbraco.Look.Models
{
    public class LocationQuery
    {
        /// <summary>
        /// The location to calculate distance from - when null = feature disabled
        /// </summary>
        public Location Location { get; set; } = null;

        /// <summary>
        ///
        /// </summary>
        public Distance MaxDistance { get; set; } = null;

        ///// <summary>
        ///// Constructor - used to ensure both location & max distance supplied
        ///// </summary>
        ///// <param name="location"></param>
        ///// <param name="maxDistance"></param>
        //public LocationQuery(Location location, Distance maxDistance = null)
        //{
        //	this.Location = location;
        //	this.MaxDistance = maxDistance;
        //}

        public override bool Equals(object obj)
        {
            var locationQuery = obj as LocationQuery;

            return locationQuery != null
                && ((locationQuery.Location == null && this.Location == null)
                    || (locationQuery.Location != null && this.Location.Equals(locationQuery)))
                && ((locationQuery.MaxDistance == null && this.MaxDistance == null)
                    || locationQuery.MaxDistance != null && this.Location.Equals(this.MaxDistance));
        }

        internal LocationQuery Clone()
        {
            var clone = (LocationQuery)this.MemberwiseClone();

            clone.Location = this.Location?.Clone();
            clone.MaxDistance = this.MaxDistance?.Clone();

            return clone;
        }
    }
}
