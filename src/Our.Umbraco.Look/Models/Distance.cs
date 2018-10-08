using System;

namespace Our.Umbraco.Look.Models
{
    public class Distance
    {
        public int DistanceValue { get; set; }

        public DistanceUnit DistanceUnit { get; set; }

        /// <summary>
        /// Constructor - used to ensure both value & unit supplied
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public Distance(int value, DistanceUnit unit)
        {
            this.DistanceValue = value;
            this.DistanceUnit = unit;
        }

        internal double GetMiles()
        {
            switch (this.DistanceUnit)
            {
                case DistanceUnit.Kilometres: return this.DistanceValue * 0.621371;
                case DistanceUnit.Miles: return this.DistanceValue;
            }

            throw new Exception();
        }
    }
}
