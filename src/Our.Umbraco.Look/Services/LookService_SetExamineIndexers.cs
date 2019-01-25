using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        internal static void SetExamineIndexers(string[] examineIndexers)
        {
            LogHelper.Info(typeof(LookService), "Examine indexers to hook into set");

            LookService.Instance.ExamineIndexers = examineIndexers;
        }
    }
}
