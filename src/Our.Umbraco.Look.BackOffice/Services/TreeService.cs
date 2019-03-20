using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Models.Tree;
using System.Linq;
using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Services
{
    internal static class TreeService
    {
        /// <summary>
        /// Factory method to make a type of LookTreeNode from the supplied (hyphen delimited) id
        /// </summary>
        /// <param name="id">"-1" or hyphen delimited, first part the node type, the 2nd a value/id</param>
        internal static ILookTreeNode MakeLookTreeNode(string id, FormDataCollection queryStrings)
        {
            if (id == "-1") return new RootTreeNode(id, queryStrings);

            var nodeType = id.Split('-')[0];
            var nodeParams = id.Split('-')[1]; // TODO: FIX: everything after the first hyphen

            switch (nodeType)
            {
                case "searcher":
                    queryStrings.ReadAsNameValueCollection()["searcherName"] = nodeParams;

                    return new SearcherTreeNode(queryStrings);

                case "nodes":
                    queryStrings.ReadAsNameValueCollection()["searcherName"] = nodeParams;

                    return new NodesTreeNode(queryStrings);

                case "nodeType":
                    var nodeTypeParams = nodeParams.Split('|');

                    queryStrings.ReadAsNameValueCollection()["searcherName"] = nodeTypeParams[0];
                    queryStrings.ReadAsNameValueCollection()["nodeType"] = nodeTypeParams[1];

                    return new NodeTypeTreeNode(queryStrings);

                case "detached":
                    var detachedParams = nodeParams.Split('|');

                    queryStrings.ReadAsNameValueCollection()["searcherName"] = detachedParams[0];
                    queryStrings.ReadAsNameValueCollection()["nodeType"] = detachedParams[1];

                    return new DetachedTreeNode(queryStrings);

                case "tags":                    
                    queryStrings.ReadAsNameValueCollection()["searcherName"] = nodeParams;

                    return new TagsTreeNode(queryStrings);

                case "tagGroup":
                    var tagGroupParams = nodeParams.Split('|');

                    queryStrings.ReadAsNameValueCollection()["searcherName"] = tagGroupParams[0];
                    queryStrings.ReadAsNameValueCollection()["tagGroup"] = tagGroupParams[1];

                    return new TagGroupTreeNode(queryStrings);

                case "tag":
                    var tagParams = nodeParams.Split('|').Take(3).ToArray(); // limit chop, as tag name may contain delimiter

                    queryStrings.ReadAsNameValueCollection()["searcherName"] = tagParams[0];
                    queryStrings.ReadAsNameValueCollection()["tagGroup"] = tagParams[1];
                    queryStrings.ReadAsNameValueCollection()["tagName"] = tagParams[2];

                    return new TagTreeNode(queryStrings); // create tag node without count

                case "locations":
                    queryStrings.ReadAsNameValueCollection()["searcherName"] = nodeParams;

                    return new LocationsTreeNode(queryStrings);
            }

            return null;
        }
    }
}
