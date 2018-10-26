using System.Linq;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class StringExtensions
    {
        /// <summary>
        /// validate that a string makes a valid tag
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsValidTag (this string value)
        {
            if (value.Any(char.IsUpper)) { return false; } // tag must be lowercase until field queried without analyzer
            if (value.Any(char.IsWhiteSpace)) { return false; }
            if (value.Contains("\\")) return false; // reserved for future use (tag tree)

            return true;
        }
    }
}
