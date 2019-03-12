using System;
using System.Text.RegularExpressions;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// A LookTag can be any string value, in an optionally named group
    /// </summary>
    public class LookTag
    {
        private const char DELIMITER = ':';

        private string _group = string.Empty; // default value (the 'name-less' default group)

        private string _name = null;

        /// <summary>
        /// Optional group for this tag
        /// </summary>
        public string Group
        {
            get
            {
                return this._group;
            }

            private set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var regex = new Regex("^[a-zA-Z0-9_]*$");

                    if (regex.IsMatch(value) && value.Length < 50)
                    {
                        this._group = value;
                    }
                    else
                    {
                        throw new Exception($"Invalid tag group '{ value }' - must be less than 50 chars and only contain alphanumberic/underscore chars");
                    }
                }
            }
        }

        /// <summary>
        /// Name of this tag (the key)
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }

            private set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this._name = value;
                }
                else
                {
                    throw new Exception("Invalid tag name, must have a value");
                }
            }
        }

        ///// <summary>
        ///// TODO: empty public constructor as might be useful for consumer mapping / serialization
        ///// </summary>
        //public LookTag()
        //{
        //}

        /// <summary>
        /// <para>Create a new LookTag from a raw string value.</para>
        /// <para>A tag can be any string value and can be in a named group.</para>
        /// <para>The raw string value will be parsed for the first colon char ':' to split a group from a tag.</para>
        /// <para>(to use a colon char in the tag string without a group, prefix the value with with the delimiting colon)</para>
        /// <para>eg.</para>
        /// <para>&#160;</para>                
        /// <para>"tag" = the tag "tag" in the group ""</para>
        /// <para>"group:tag" = the tag "tag" in the group "group"</para>
        /// <para>":tag:colon" = the tag "tag:colon" in the group ""</para>
        /// </summary>
        /// <param name="value">the raw string value for a tag (with optional group)</param>
        public LookTag(string value)
        {
            var delimiter = value.IndexOf(DELIMITER);

            if (delimiter > -1)
            {
                this.Group = value.Substring(0, delimiter);
                this.Name = value.Substring(delimiter + 1);
            }
            else
            {
                this.Name = value;
            }
        }

        /// <summary>
        /// Create a new LookTag from specifed group and tag name values
        /// </summary>
        /// <param name="group">name of tag group, a null or string.Empty indicate this tag belongs to the default 'name-less' group</param>
        /// <param name="name">the unique name for this tag within this tag group (all chars valid)</param>
        public LookTag(string group, string name)
        {
            this.Group = group;
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.ToString() == obj.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 0;

                hashCode = (hashCode * 397) ^ this.Group.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Name.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        /// Serialize using the colon char delimiter to split the group from the tag
        /// </summary>
        /// <returns>delimited string value for group:tag</returns>
        public override string ToString()
        {
            return this.Group + DELIMITER + this.Name;
        }
    }
}
