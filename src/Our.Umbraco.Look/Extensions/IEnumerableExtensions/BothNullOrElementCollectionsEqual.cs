using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Look.Extensions
{
    internal static partial class IEnumerableExtensions
    {
        /// <summary>
        /// Returns true if both are null, or both collections of collections have the same elements in each regardless of sequence
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        internal static bool BothNullOrElementCollectionsEqual<TSource>(this IEnumerable<IEnumerable<TSource>> first, IEnumerable<IEnumerable<TSource>> second)
        {
            if (first == null && second == null) return true;

            if (first == null || second == null) return false;

            if (first.Count() != second.Count()) return false;

            var areEqual = true;

            if (first.Any())
            {
                var firstStack = new Stack<IEnumerable<TSource>>(first);
                var secondList = new List<IEnumerable<TSource>>(second);

                do
                {
                    var firstCollection = firstStack.Pop();

                    var matchingCollectionFound = false;

                    var secondStack = new Stack<IEnumerable<TSource>>(secondList);

                    do
                    {
                        var secondCollection = secondStack.Pop();

                        matchingCollectionFound = firstCollection.BothNullOrElementsEqual(secondCollection);

                        if (matchingCollectionFound)
                        {
                            secondList.Remove(secondCollection);
                        }
                    }
                    while (!matchingCollectionFound && secondStack.Any());

                    areEqual = matchingCollectionFound;
                }
                while (areEqual && firstStack.Any());
            }

            return areEqual;
        }
    }
}
