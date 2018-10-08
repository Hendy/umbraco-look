namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Currently handles it's own serialization / de-serialization
    /// </summary>
    public class Location
    {
        /// <summary>
        /// 
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// 
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Constructor - used to ensure both lat & lng supplied (neither are then changeable)
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public Location(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Constructor - internal helper for de-serialization
        /// </summary>
        /// <param name="location"></param>
        internal Location(string location)
        {
            try
            {
                this.Latitude = double.Parse(location.Split('|')[0]);
                this.Longitude = double.Parse(location.Split('|')[1]);
            }
            catch
            {
            }
        }

        /// <summary>
        /// serialization helper
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Latitude.ToString() + "|" + this.Longitude.ToString();
        }
    }
}
