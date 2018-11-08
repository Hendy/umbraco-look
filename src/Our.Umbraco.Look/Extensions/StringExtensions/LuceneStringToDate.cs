using Lucene.Net.Documents;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class StringExtensions
    {
        /// <summary>
        /// Helper to convert from a string value stored in Lucene to a DateTime?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static DateTime? LuceneStringToDate(this string value)
        {
            DateTime? date = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    date = DateTools.StringToDate(value);
                }
                catch
                {
                    LogHelper.Info(typeof(DateTimeExtensions), $"Unable to convert string '{value}' into a DateTime");
                }
            }

            return date;
        }
    }
}
