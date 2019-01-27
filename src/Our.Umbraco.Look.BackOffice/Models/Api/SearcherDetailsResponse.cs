using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Look.BackOffice.Models.Api
{
    /// <summary>
    /// Response POCO for the ApiController.GetSearcherDetails() method
    /// </summary>
    public class SearcherDetailsResponse
    {
        public string WhatCaseAmI { get; set; } = "default";

    }
}
