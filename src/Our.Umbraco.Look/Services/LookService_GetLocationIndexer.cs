using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static Func<IndexingContext, Location> GetLocationIndexer()
        {
            return LookService.Instance._locationIndexer;
        }
    }
}
