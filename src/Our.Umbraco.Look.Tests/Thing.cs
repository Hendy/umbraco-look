using Our.Umbraco.Look.Models;
using System;

namespace Our.Umbraco.Look.Tests
{
    /// <summary>
    /// POCO used to index & query - it has a property for each custom field
    /// </summary>
    internal class Thing
    {
        internal string Name { get; set; }

        internal DateTime? Date { get; set; }

        internal string Text { get; set; }

        internal LookTag[] Tags { get; set; }

        internal Location Location { get; set; }
    }
}
