using System;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Location model, used to specify a latitude and longitude
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var location = obj as Location;

            return location != null
                && location.ToString() == this.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Location Clone()
        {
            return (Location)this.MemberwiseClone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
