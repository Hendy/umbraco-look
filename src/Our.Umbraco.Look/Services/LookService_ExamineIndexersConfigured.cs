namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// return the flag indicating whether the examine indexers to hook into has been configured
        /// </summary>
        internal static bool ExamineIndexersConfigured
        {
            get
            {
                return LookService.Instance._examineIndexersConfigured;
            }
        }
    }
}
