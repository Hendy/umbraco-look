using Examine.LuceneEngine.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Look
{
    public class LookDetachedIndexer : LuceneIndexer
    {


        protected override void PerformIndexAll(string type)
        {
            
            
        }

        protected override void PerformIndexRebuild()
        {
            // triggered from a back office rebuild - but UI get's lost
        }
    }
}
