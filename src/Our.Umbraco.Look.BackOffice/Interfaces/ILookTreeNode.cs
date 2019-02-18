using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.Look.BackOffice.Interfaces
{
    internal interface ILookTreeNode
    {
        string Id { get; }

        FormDataCollection QueryStrings { get; }

        string Name { get; }

        string Icon { get; } // icon class name

        /// <summary>
        /// The url part to the resource
        /// </summary>
        string RoutePath { get; }

        /// <summary>
        /// Child tree nodes
        /// </summary>
        /// <returns></returns>
        ILookTreeNode[] GetChildren();

        /// <summary>
        /// Umbraco tree menu items
        /// </summary>
        /// <returns></returns>
        MenuItemCollection GetMenu();
    }
}
