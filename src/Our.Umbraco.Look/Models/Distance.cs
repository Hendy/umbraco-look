using System;

namespace Our.Umbraco.Look.Models
{
    public class Distance
    {
        public int DistanceValue { get; set; }

        public DistanceUnit DistanceUnit { get; set; }

        /// <summary>
        /// Create a new Distance model
        /// </summary>
        /// <param name="value">An integer value</param>
        /// <param name="unit">Kilometers or Miles</param>
        public Distance(int value, DistanceUnit unit)
        {
            this.DistanceValue = value;
            this.DistanceUnit = unit;
        }

        public override bool Equals(object obj)
        {
            var distance = obj as Distance;

            return distance != null
                && distance.DistanceValue == this.DistanceValue
                && distance.DistanceUnit == this.DistanceUnit;
        }

        internal Distance Clone()
        {
            var clone = (Distance)this.MemberwiseClone();

            clone.DistanceUnit = this.DistanceUnit;

            return clone;
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
