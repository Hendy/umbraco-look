﻿using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Register consumer code to perform when indexing date
        /// </summary>
        /// <param name="dateIndexer">Your custom date indexing function</param>
        internal static void SetDateIndexer(Func<IndexingContext, DateTime?> dateIndexer)
        {
            LogHelper.Info(typeof(LookService), "Date indexing function set");
            
            LookService.Instance._dateIndexer = dateIndexer;
        }
    }
}
