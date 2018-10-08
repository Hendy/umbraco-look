using System.Collections.Generic;

namespace Our.Umbraco.Look.Interfaces
{
    /// <summary>
    /// Used to set a known total with an enumerable (this is to avoid iteration though the enumerable to get at the total)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnumerableWithTotal<out T> : IEnumerable<T>
    {
        /// <summary>
        /// The total number of items expected in the enumerable
        /// </summary>
        int Total { get; }
    }
}
