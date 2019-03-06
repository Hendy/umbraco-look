using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, string> GetTextIndexer()
        {
            return LookService.Instance._textIndexer;
        }
    }
}
