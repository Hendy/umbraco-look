using System;
using System.Text.RegularExpressions;

namespace Our.Umbraco.Look.Models
{
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
        /// Constructor - create a tag in the default 'name-less' group
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
        /// Constructor - create a tag in the specified group
        /// </summary>
        /// <param name="group">name of tag group, a null or string.Empty indicate this tag belongs to the default 'name-less' group</param>
        /// <param name="name">the unique name for this tag within this tag group (all chars valid)</param>
        public LookTag(string group, string name)
        {
            this.Group = group;
            this.Name = name;
        }

        public override bool Equals(object obj)
        {
            return this.ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Group + DELIMITER + this.Name;
        }
    }
}
