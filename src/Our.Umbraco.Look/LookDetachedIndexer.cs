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
            //throw new NotImplementedException();
        }

        protected override void PerformIndexRebuild()
        {
            //throw new NotImplementedException();
        }
    }
}
