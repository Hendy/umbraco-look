namespace Our.Umbraco.Look.Extensions
{
    internal static partial class ObejctExtensions
    {
        /// <summary>
        /// Returns true if both are null, or they equal
        /// (helper to simplify the null checking in the consumers)
        /// </summary>
        internal static bool BothNullOrEquals(this object first, object second)
        {
            if (first == null && second == null) return true;

            if (first == null || second == null) return false; // one exists, so the other must be null

            return first.Equals(second);
        }
    }
}
