using Examine.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look.BackOffice.Services
{
    /// <summary>
    /// common place for static helpers to get css class names for icons
    /// </summary>
    internal static class IconService
    {
        /// <summary>
        /// The icon for a searcher varies dependent on its state
        /// </summary>
        /// <param name="searcher"></param>
        /// <returns></returns>
        internal static string GetSearcherIcon(BaseSearchProvider searcher)
        {
            if (searcher is LookSearcher)
            {
                return "icon-files";
            }
            else // must be an examine one
            {
                var name = searcher.Name.TrimEnd("Searcher");

                if (LookConfiguration.ExamineIndexers.Select(x => x.TrimEnd("Indexer")).Any(x => x == name))
                {
                    return "icon-categories";
                }
                else // not hooked in 
                {
                    return "icon-file-cabinet";
                }
            }

        }

        internal static string GetNodeTypeIcon(PublishedItemType nodeType)
        {
            return nodeType == PublishedItemType.Content ? "icon-umb-content"
            : nodeType == PublishedItemType.Media ? "icon-umb-media"
            : nodeType == PublishedItemType.Member ? "icon-umb-members"
            : null;
        }
    }
}
