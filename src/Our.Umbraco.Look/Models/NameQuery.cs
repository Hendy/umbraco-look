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
                if (!this.ContainsWildcards(value))
                {
                    this._startsWith = value;
                }
                else
                {
                    throw new Exception($"StartsWith value must not contain any wildcard characters '*', '?'");
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
                if (!this.ContainsWildcards(value))
                {
                    this._endsWith = value;
                }
                else
                {
                    throw new Exception($"EndsWith value must not contain any wildcard characters '*', '?'");
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
                if (!this.ContainsWildcards(value))
                {
                    this._contains = value;
                }
                else
                {
                    throw new Exception($"Contains value must not contain any wildcard characters '*', '?'");
                }
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool CaseSensitive { get; set; } = true;

        private bool ContainsWildcards(string value)
        {
            return !value.Contains("*") && !value.Contains("?");
        }
    }
}
