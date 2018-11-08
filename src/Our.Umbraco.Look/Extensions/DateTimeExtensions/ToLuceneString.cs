using Lucene.Net.Documents;
using System;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class DateTimeExtensions
    {
        /// <summary>
        /// Helper to convert a DateTime into a string suitable for Lucene to store
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static string ToLuceneString(this DateTime? dateTime)
        {
            if (dateTime != null)
            {
                return dateTime.Value.ToLuceneString();
            }

            return null;
        }

        /// <summary>
        /// Helper to convert a DateTime into a string suitable for Lucene to store
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static string ToLuceneString(this DateTime dateTime)
        {
            return DateTools.DateToString(dateTime, DateTools.Resolution.SECOND);
        }
    }
}
