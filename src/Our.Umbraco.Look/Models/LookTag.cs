using System;
using System.Collections.Generic;

namespace Our.Umbraco.Look.Models
{
    public class LookTag : IEqualityComparer<LookTag>
    {
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
                    var valid = value.Length < 50 // artifical limit as this is used a lucene field name
                                && !value.Contains(" ")
                                && !value.Contains(".");

                    if (valid)
                    {
                        this._group = value;
                    }
                    else
                    {
                        throw new Exception($"Invalid tag group '{ value }' - must be less than 50 chars and not contain whitespace nor '.'");
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

        /// <summary>
        /// Constructor - create a tag in the default 'name-less' group
        /// </summary>
        /// <param name="name">the unique name for this tag (all chars valid)</param>
        public LookTag(string name)
        {
            this.Name = name;
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

        public bool Equals(LookTag x, LookTag y)
        {
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(LookTag obj)
        {
            return obj.GetHashCode();
        }

        public override string ToString()
        {
            return this.Group + "|" + this.Name;
        }

        internal static LookTag FromString(string value)
        {
            LookTag tag = null;

            var pipe = value.IndexOf('|');

            if (pipe > -1)
            {
                var group = value.Substring(0, pipe);
                var name = value.Substring(pipe + 1);

                tag = new LookTag(group, name);
            }
            else
            {
                throw new Exception($"Unable to deserialize string '{ value }' into a Tag object");
            }

            return tag;
        }
    }
}
