using System;

namespace Our.Umbraco.Look
{
    public class LocationBoundary
    {
        public double LatitudeMin { get; }

        public double LatitudeMax { get; }

        public double LongitudeMin { get; }

        public double LongitudeMax { get; }

        //public LocationBoundary(double latitudeMin, double latitudeMax, double longitudeMin, double longitudeMax)
        //{
        //}

        /// <summary>
        /// Construct with two points, each of which indicating opposing corners
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
