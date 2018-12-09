using System;

namespace Our.Umbraco.Look
{
    public static class LookConfiguration
    {
        public static Func<IndexingContext, string> NameIndexer { set { LookService.SetNameIndexer(value); } }

        public static Func<IndexingContext, DateTime?> DateIndexer { set { LookService.SetDateIndexer(value); } }

        public static Func<IndexingContext, string> TextIndexer { set { LookService.SetTextIndexer(value); } }

        public static Func<IndexingContext, LookTag[]> TagIndexer { set { LookService.SetTagIndexer(value); } }

        public static Func<IndexingContext, Location> LocationIndexer { set { LookService.SetLocationIndexer(value); } }

        public static RequestFields RequestFields { set { LookService.SetRequestFields(value); } }
    }
}
