using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, bool> GetIndexIf()
        {
            return LookService.Instance._indexIf;
        }
    }
}
