using Examine;
using System.Linq;
using UmbracoExamine;

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        internal static BaseUmbracoIndexer[] GetExamineIndexers()
        {
            return ExamineManager
                        .Instance
                        .IndexProviderCollection
                        .Select(x => x as BaseUmbracoIndexer) // UmbracoContentIndexer, UmbracoMemberIndexer
                        .Where(x => x != null)
                        .Where(x => LookService.Instance.ExamineIndexers == null || LookService.Instance.ExamineIndexers.Contains(x.Name))
                        .ToArray();
        }
    }
}
