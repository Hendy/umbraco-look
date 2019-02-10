using System;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    /// <summary>
    /// Respesents a matched document in the indexed (subset of a LookMatch)
    /// this is for api serialzation
    /// </summary>
    public class Match
    {
        Guid Key { get; set; }

        // id, key
        // content,media,member
        // tags

        // link
        // name
    }
}
