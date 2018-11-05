using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Models
{
    public class Tag
    {
        /// <summary>
        /// Optional group for this tag
        /// </summary>
        public string Group { get; } = string.Empty;

        /// <summary>
        /// Name of this tag (the key)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor - create a tag in the default 'name-less' group
        /// </summary>
        /// <param name="name">the unique name for this tag (all chars valid)</param>
        public Tag(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Constructor - create a tag in the specified group
        /// </summary>
        /// <param name="group">name of tag group, a null or string.Empty indicate this tag belongs to the default 'name-less' group</param>
        /// <param name="name">the unique name for this tag within this tag group (all chars valid)</param>
        public Tag(string group, string name)
        {
            this.Name = name;

            if (!string.IsNullOrWhiteSpace(group))
            {
                var valid = group.Length > 100 // artifical limit as this is used a lucene field name
                            || !group.Any(x => char.IsWhiteSpace(x) || x == '.');

                if (valid)
                {
                    this.Group = group;
                }
                else
                {
                    LogHelper.Debug(typeof(Tag), $"Invalid tag group '{ group }' - must be less than 100 chars and not contain whitespace nor '.'");
                }
            }
        }
    }
}
