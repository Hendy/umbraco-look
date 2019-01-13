using System;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class GuidExtensions
    {
        /// <summary>
        /// Helper to convert a Guid into a string suitable for Lucene to store
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static string GuidToLuceneString(this Guid? guid)
        {
            if (guid != null)
            {
                return guid.Value.GuidToLuceneString();
            }

            return null;
        }

        /// <summary>
        /// Helper to convert a Guid into a string suitable for Lucene to store
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static string GuidToLuceneString(this Guid guid)
        {
            return guid.ToString("N");
        }
    }
}
