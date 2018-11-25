using System;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Currently handles it's own serialization / de-serialization
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The longitude
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// The longitude
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Constructor - used to ensure both latitude and longitude supplied (neither are then changeable)
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public Location(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// serialization helper
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Latitude.ToString() + "|" + this.Longitude.ToString();
        }

        public override bool Equals(object obj)
        {
            var locationQuery = obj as LocationQuery;

            return locationQuery != null
                && locationQuery.ToString() == this.ToString();
        }

        internal Location Clone()
        {
            return (Location)this.MemberwiseClone();
        }

        internal static Location FromString(string value)
        {
            Location location = null;

            try
            {
                var latitude = double.Parse(value.Split('|')[0]);
                var longitude = double.Parse(value.Split('|')[1]);

                location = new Location(latitude, longitude);
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to deserialize string '{value}' into a Location ojbect", exception);
            }

            return location;
        }
    }
}
