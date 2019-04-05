using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, bool> GetBeforeIndexing()
        {
            return LookService.Instance._beforeIndexing;
        }
    }
}
