using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class StringExtensions
    {
        /// <summary>
        /// Helper to convert from a string value stored in Lucene to a Guid?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static Guid? LuceneStringToGuid(this string value)
        {
            if (Guid.TryParseExact(value, "N", out Guid guid))
            {
                return guid;
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                LogHelper.Warn(typeof(DateTimeExtensions), $"Unexpected value found - unable to convert string '{value}' (using format 'N') into a Guid");
            }

            return null;
        }
    }
}
