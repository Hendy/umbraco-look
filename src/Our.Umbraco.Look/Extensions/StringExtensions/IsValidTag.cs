using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class StringExtensions
    {
        /// <summary>
        /// Validate that a string makes a valid tag
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsValidTag (this string value)
        {
            var isValid = true;

            //if (value.Any(char.IsUpper)) { isValid = false; } // tag must be lowercase until field queried without analyzer
            //if (value.Any(char.IsWhiteSpace)) { isValid = false; }
            if (value.Contains("\\")) isValid = false; // reserved for future use (tag tree)

            if (!isValid)
            {
                LogHelper.Debug(typeof(StringExtensions), $"Invalid string value '{ value }' for a tag");
            }

            return isValid;
        }
    }
}
