using System;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Specify a pane on latitude/longitude axis (typically boundary of rendered map)
    /// </summary>
    public class LocationBoundary
    {
        /// <summary>
        /// Southernmost coordinate
        /// </summary>
        public double LatitudeMin { get; }

        /// <summary>
        /// Northenmost coordinate
        /// </summary>
        public double LatitudeMax { get; }

        /// <summary>
        /// Westernmost coordinate
        /// </summary>
        public double LongitudeMin { get; }

        /// <summary>
        /// Easternmost coordinate
        /// </summary>
        public double LongitudeMax { get; }

        /// <summary>
        /// Construct with two points, each indicating diagonally opposing corners
        /// </summary>
        /// <param name="locationOne"></param>
        /// <param name="locationTwo"></param>
        public LocationBoundary(Location locationOne, Location locationTwo)
        {
            this.LatitudeMin = Math.Min(locationOne.Latitude, locationTwo.Latitude);
            this.LatitudeMax = Math.Max(locationOne.Latitude, locationTwo.Latitude);

            this.LongitudeMin = Math.Min(locationOne.Longitude, locationTwo.Longitude);
            this.LongitudeMax = Math.Max(locationOne.Longitude, locationTwo.Longitude);
        }
    }
}
