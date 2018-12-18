using Examine.LuceneEngine.Providers;
using System;

namespace Our.Umbraco.Look
{
    public class LookDetachedIndexer : LuceneIndexer
    {
        protected override void PerformIndexAll(string type)
        {
        }

        protected override void PerformIndexRebuild()
        {
            // triggered from a back office rebuild 
            this.OnIndexOperationComplete(new EventArgs()); // causes the backoffice rebuild to end
        }
    }
}
