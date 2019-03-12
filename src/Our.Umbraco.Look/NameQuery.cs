using Lucene.Net.QueryParsers;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// Search query criteria for use with the custom name field
    /// </summary>
    public class NameQuery
    {
        private string _is;

        private string _startsWith;

        private string _contains;

        private string _endsWith;

        public string Is
        {
            get
            {
                return this._is;
            }
            set
            {
                if (value != null)
                {
                    this._is = QueryParser.Escape(value);
                }
                else
                {
                    this._is = null;
                }
            }
        }

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
                if (value != null)
                {
                    this._startsWith = QueryParser.Escape(value);
                }
                else
                {
                    this._startsWith = null;
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
                if (value != null)
                {
                    this._contains = QueryParser.Escape(value);
                }
                else
                {
                    this._contains = null;
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
                if (value != null)
                {
                    this._endsWith = QueryParser.Escape(value);
                }
                else
                {
                    this._endsWith = null;
                }
            }
        }

        /// <summary>
        /// When true, StartsWith, EndsWith or Contains critera properties will be case sensitive
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            NameQuery nameQuery = obj as NameQuery;

            return nameQuery != null
                && nameQuery.Is == this.Is
                && nameQuery.StartsWith == this.StartsWith
                && nameQuery.Contains == this.Contains
                && nameQuery.EndsWith == this.EndsWith
                && nameQuery.CaseSensitive == this.CaseSensitive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal NameQuery Clone()
        {
            return (NameQuery)this.MemberwiseClone();
        }
    }
}
