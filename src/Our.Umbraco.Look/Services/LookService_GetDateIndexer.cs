using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, DateTime?> GetDateIndexer()
        {
            return LookService.Instance._dateIndexer;
        }
    }
}
