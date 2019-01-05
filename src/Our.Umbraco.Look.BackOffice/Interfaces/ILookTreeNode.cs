namespace Our.Umbraco.Look.BackOffice.Interfaces
{
    internal interface ILookTreeNode
    {
        string Id { get; }

        string Name { get; }

        string Icon { get; } // icon class name

        ILookTreeNode[] Children { get; }
    }
}
