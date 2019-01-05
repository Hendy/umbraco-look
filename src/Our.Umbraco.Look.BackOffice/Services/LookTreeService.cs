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

            switch (chopped[0])
            {
                case "searcher": return new SearcherTreeNode(ExamineManager.Instance.SearchProviderCollection[chopped[1]]);
                case "tags": return new TagsTreeNode(ExamineManager.Instance.SearchProviderCollection[chopped[1]]);
            }

            return null;
        }

    }
}
