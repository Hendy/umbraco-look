using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, string> GetNameIndexer()
        {
            return LookService.Instance._nameIndexer;
        }
    }
}
