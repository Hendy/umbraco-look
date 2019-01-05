using Examine;
using Our.Umbraco.Look.BackOffice.Interfaces;
using Our.Umbraco.Look.BackOffice.Models;

namespace Our.Umbraco.Look.BackOffice.Services
{
    internal static class LookTreeService
    {
        /// <summary>
        /// Factory method to make a type of LookTreeNode from the supplied (hyphen delimited) id
        /// </summary>
        /// <param name="id"></param>
        internal static ILookTreeNode MakeLookTreeNode(string id)
        {
            if (id == "-1")
            {
                return new RootTreeNode(id);
            }

            var chopped = id.Split('-');
            var nodeType = chopped[0];
            var nodeValue = chopped[1];

            switch (nodeType)
            {
                case "searcher": return new SearcherTreeNode(ExamineManager.Instance.SearchProviderCollection[nodeValue]);
                case "tags": return new TagsTreeNode(nodeValue);
                case "tagGroup": return new TagGroupTreeNode(nodeValue.Split('|')[0], nodeValue.Split('|')[1]);
                //case "tag": return new TagTreeNode(nodeValue.Split('|')
            }

            return null;
        }

    }
}
