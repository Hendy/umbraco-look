using Our.Umbraco.Look.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Our.Umbraco.Look.Models
{
    public class EnumerableWithTotal<T> : IEnumerableWithTotal<T>
    {
        /// <summary>
        /// Implementation of the the IEnumerableWithTotal{T} interface
        /// </summary>
        int IEnumerableWithTotal<T>.Total => this.total;

        /// <summary>
        /// The wrapped enumerable collection (as supplied in the constructor)
        /// </summary>
        private IEnumerable<T> enumerable;

        /// <summary>
        /// The expected total number of items in the enumerable
        /// </summary>
        private int total;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumerable">The enumerable collection</param>
        /// <param name="total">The total number of items expected in the enumerable collection</param>
        internal EnumerableWithTotal(IEnumerable<T> enumerable, int total)
        {
            this.enumerable = enumerable;
            this.total = total;
        }

        /// <summary>
        /// Implementation of the IEnumerable{out T} interface
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.enumerable.GetEnumerator();
        }

        /// <summary>
        /// Implementation of the the IEnumerable interface
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
