namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// Get the default request fields value
        /// </summary>
        /// <returns></returns>
        internal static RequestFields GetRequestFields()
        {
            return LookService.Instance.RequestFields;
        }
    }
}
