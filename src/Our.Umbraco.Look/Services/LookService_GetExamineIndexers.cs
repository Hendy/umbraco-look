using System.Linq;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get all Examine indexers that Look is currently hooked into (this collection doesn't include the Look indexer/searcher, only the Examine ones)
        /// </summary>
        /// <returns></returns>
        internal static string[] GetExamineIndexers()
        {
            return LookService.Instance._examineDocumentWritingEvents.Select(x => x.Key).ToArray();
        }
    }
}
