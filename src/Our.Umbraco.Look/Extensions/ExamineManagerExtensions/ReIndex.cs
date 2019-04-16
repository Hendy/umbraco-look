using Examine;
using Our.Umbraco.Look.Extensions;
using System.Collections.Generic;
using System.Linq;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Models;
using Umbraco.Web;
using UmbracoExamine;
using Media = umbraco.cms.businesslogic.media.Media;
using Member = umbraco.cms.businesslogic.member.Member;

namespace Our.Umbraco.Look // NOTE: namespaced pushed down to root as it's in the public API
{
    /// <summary>
    /// Look extension method for ExmaineManager to reindex nodes by id or IPublishedContent collections
    /// </summary>
    public static partial class ExamineManagerExtensions
    {
        public static void ReIndex(this ExamineManager examineManager, IEnumerable<int> ids)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            var nodes = ids
                        .Select(x => umbracoHelper.GetIPublishedContent(x))
                        .Where(x => x != null);

            examineManager.ReIndex(nodes);
        }

        public static void ReIndex(this ExamineManager examineManager, IEnumerable<IPublishedContent> nodes)
        {
            var examineIndexers = examineManager
                                        .IndexProviderCollection
                                        .Select(x => x as BaseUmbracoIndexer) // UmbracoContentIndexer, UmbracoMemberIndexer
                                        .Where(x => x != null);

            foreach (var examineIndexer in examineIndexers)
            {
                foreach(var node in nodes)
                {
                    switch (node.ItemType)
                    {
                        case PublishedItemType.Content:
                            examineIndexer.ReIndexNode(new Document(node.Id).ToXDocument(false).Root, IndexTypes.Content);
                            break;

                        case PublishedItemType.Media:
                            examineIndexer.ReIndexNode(new Media(node.Id).ToXDocument(false).Root, IndexTypes.Media);
                            break;

                        case PublishedItemType.Member:
                            examineIndexer.ReIndexNode(new Member(node.Id).ToXDocument(false).Root, IndexTypes.Member);
                            break;                                
                    }
                }
            }

            var lookIndexers = examineManager
                                .IndexProviderCollection
                                .Select(x => x as LookIndexer)
                                .Where(x => x != null);

            foreach (var lookIndexer in lookIndexers)
            {
                lookIndexer.ReIndex(nodes);
            }

        }
    }
}
