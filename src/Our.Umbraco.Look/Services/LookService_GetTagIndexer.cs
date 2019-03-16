using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, LookTag[]> GetTagIndexer()
        {
            return LookService.Instance._tagIndexer;
        }
    }
}
