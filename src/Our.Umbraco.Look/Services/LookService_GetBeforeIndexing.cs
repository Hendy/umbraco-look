using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Action<IndexingContext> GetBeforeIndexing()
        {
            return LookService.Instance._beforeIndexing ?? new Action<IndexingContext>(x => { });
        }
    }
}
