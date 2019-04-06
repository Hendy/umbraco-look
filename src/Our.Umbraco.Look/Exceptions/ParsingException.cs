using System;

namespace Our.Umbraco.Look.Exceptions
{
    internal class ParsingException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        internal ParsingException(string message) : base(message)
        {
        }
    }
}
