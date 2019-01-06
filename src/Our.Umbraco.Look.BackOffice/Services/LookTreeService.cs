using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Models;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Services
{
    internal static class LookTreeService
    {
        /// <summary>
        /// Factory method to make a type of LookTreeNode from the supplied (hyphen delimited) id
        /// </summary>
        /// <param name="id">"-1" or hyphen delimited, first part the node type, the 2nd a value/id</param>
        internal static ILookTreeNode MakeLookTreeNode(string id, FormDataCollection queryStrings)
        {
            if (id == "-1") return new RootTreeNode(id, queryStrings);

            var nodeType = id.Split('-')[0];
            var nodeParams = id.Split('-')[1]; // FIX: everything after the first hyphen -

            switch (nodeType)
            {
                case "searcher":
                    queryStrings.ReadAsNameValueCollection()["searcherName"] = nodeParams;

                    return new SearcherTreeNode(queryStrings);

                case "tags":
                    
                    queryStrings.ReadAsNameValueCollection()["searcherName"] = nodeParams;

                    return new TagsTreeNode(queryStrings);

                case "tagGroup":
                    var tagGroupParams = nodeParams.Split('|');

                    queryStrings.ReadAsNameValueCollection()["searcherName"] = tagGroupParams[0];
                    queryStrings.ReadAsNameValueCollection()["tagGroup"] = tagGroupParams[1];

                    return new TagGroupTreeNode(queryStrings);

                case "tag":
                    //var tagParams = id.Split('-')

                    return new TagTreeNode(queryStrings);
            }

            return null;
        }

    }
}
