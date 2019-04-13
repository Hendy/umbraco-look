using Our.Umbraco.Look.Extensions;

namespace Our.Umbraco.Look
{
    public class LocationQuery
    {
        /// <summary>
        /// When set only results inside this boundary are returned (boundary expected to be an aligned to lat / lng axis)
        /// </summary>
        public LocationBoundary Boundary { get; set; } = null;

        /// <summary>
        /// The location to calculate distance from
        /// </summary>
        public Location Location { get; set; } = null;

        /// <summary>
        /// Max distance of results from the Location
        /// </summary>
        public Distance MaxDistance { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var locationQuery = obj as LocationQuery;

            return locationQuery != null
                && locationQuery.Location.BothNullOrEquals(this.Location)
                && locationQuery.MaxDistance.BothNullOrEquals(this.MaxDistance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal LocationQuery Clone()
        {
            var clone = new LocationQuery();

            clone.Location = this.Location?.Clone();
            clone.MaxDistance = this.MaxDistance?.Clone();

            return clone;
        }
    }
}
