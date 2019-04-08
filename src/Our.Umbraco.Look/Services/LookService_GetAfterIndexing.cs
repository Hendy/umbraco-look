using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Action<IndexingContext> GetAfterIndexing()
        {
            return LookService.Instance._afterIndexing ?? new Action<IndexingContext>(x => { });
        }
    }
}
