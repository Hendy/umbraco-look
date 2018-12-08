

namespace Our.Umbraco.Look
{
    public partial class LookService
    {
        /// <summary>
        /// Set the default behaviour for which Lucene fields should be returned.
        /// </summary>
        /// <param name="requestFields">The enum value specificy which fields should be returned for queries</param>
        public static void SetRequestFields(RequestFields requestFields)
        {
            LookService.Instance.RequestFields = requestFields;
        }
    }
}
