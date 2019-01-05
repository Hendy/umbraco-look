using System.Net.Http.Formatting;

namespace Our.Umbraco.Look.BackOffice.Interfaces
{
    internal interface ILookTreeNode
    {
        string Id { get; }

        FormDataCollection QueryStrings { get; }

        string Name { get; }

        string Icon { get; } // icon class name

        ILookTreeNode[] GetChildren();
    }
}
