using System;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// Search query criteria for use with the custom name field
    /// </summary>
    public class NameQuery
    {
        private string _startsWith;

        private string _endsWith;

        private string _contains;

        /// <summary>
        /// (Optional) set a string which the name must begin with
        /// </summary>
        public string StartsWith
        {
            get
            {
                return this._startsWith;
            }

            set
            {
                if (this.IsValid(value))
                {
                    this._startsWith = value;
                }
                else
                {
                    throw new Exception($"StartsWith value '{ value }' must not contain any wildcard characters '*', '?'");
                }
            }
        }

        /// <summary>
        /// (Optional) set a string which the name must end with
        /// </summary>
        public string EndsWith
        {
            get
            {
                return this._endsWith;
            }

            set
            {
                if (this.IsValid(value))
                {
                    this._endsWith = value;
                }
                else
                {
                    throw new Exception($"EndsWith value '{ value }' must not contain any wildcard characters '*', '?'");
                }
            }
        }

        /// <summary>
        /// (Optional) set a string which must be present in the name
        /// </summary>
        public string Contains
        {
            get
            {
                return this._contains;
            }

            set
            {
                if (this.IsValid(value))
                {
                    this._contains = value;
                }
                else
                {
                    throw new Exception($"Contains value '{ value }' must not contain any wildcard characters '*', '?'");
                }
            }
        }

        /// <summary>
        /// When true, StartsWith, EndsWith or Contains critera properties will be case sensitive
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// Helper to parse user set value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsValid(string value)
        {
            if (value == null)
            {
                return true;
            }

            //if (value.Contains("*") || value.Contains("?"))
            //{
            //    throw new Exception($"Value must not contain any wildcard chars '*', or '?'");
            //}

            return !value.Contains("*") 
                && !value.Contains("?");
        }
    }
}
