using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using System;
using System.Collections.Generic;

namespace Our.Umbraco.Look
{
    public class LookSearchCriteria : ISearchCriteria
    {
        #region Look search criteria extensions (using properties to distinguish from the Examine fluent API methods)

        private ISearchCriteria _examineQuery = null;
             
        /// <summary>
        /// 
        /// </summary>
        public ISearchCriteria ExamineQuery
        {
            get
            {
                return this._examineQuery;
            }
            set
            {
                if (value == null)
                {
                    this._examineQuery = null;
                }
                else if (value is LuceneSearchCriteria)
                {
                    this._examineQuery = value;
                }
                else
                {
                    throw new Exception($"Expected the supplied ISearchCriteria to be of type LuceneSearchCriteria (as would be returned by the Exmaine .Compile() method, instead got '{value?.GetType().FullName}')");
                }
            }
        } 

        /// <summary>
        /// Set an optional (and) Look NodeQuery clause
        /// </summary>
        public NodeQuery NodeQuery { get; set; }

        /// <summary>
        /// Set an optinal (and) Look NameQuery clause
        /// </summary>
        public NameQuery NameQuery { get; set; }

        /// <summary>
        /// Set an optional (and) Look DateQuery clause
        /// </summary>
        public DateQuery DateQuery { get; set; }

        /// <summary>
        /// Set an optional (and) Look TextQuery clause
        /// </summary>
        public TextQuery TextQuery { get; set; }

        /// <summary>
        /// Set an optoinal (and) Look TagQuery clause
        /// </summary>
        public TagQuery TagQuery { get; set; }

        /// <summary>
        /// Set an optional (and) Look LocationQuery clause
        /// </summary>
        public LocationQuery LocationQuery { get; set; }

        #endregion

        #region Wrapping the Examine search criteria

        /// <summary>
        /// the wrapped search criteria
        /// </summary>
        private ISearchCriteria SearchCriteria { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchCriteria">The searchCriteria to wrap</param>
        public LookSearchCriteria(ISearchCriteria searchCriteria)
        {
            this.SearchCriteria = searchCriteria;
        }

        public string SearchIndexType => this.SearchCriteria.SearchIndexType;

        public BooleanOperation BooleanOperation => this.SearchCriteria.BooleanOperation;

        public IBooleanOperation Field(string fieldName, string fieldValue)
        {
            return this.SearchCriteria.Field(fieldName, fieldValue);
        }

        public IBooleanOperation Field(string fieldName, IExamineValue fieldValue)
        {
            return this.SearchCriteria.Field(fieldName, fieldValue);
        }

        public IBooleanOperation GroupedAnd(IEnumerable<string> fields, params string[] query)
        {
            return this.SearchCriteria.GroupedAnd(fields, query);
        }

        public IBooleanOperation GroupedAnd(IEnumerable<string> fields, params IExamineValue[] query)
        {
            return this.SearchCriteria.GroupedAnd(fields, query);
        }

        public IBooleanOperation GroupedFlexible(IEnumerable<string> fields, IEnumerable<BooleanOperation> operations, params string[] query)
        {
            return this.SearchCriteria.GroupedFlexible(fields, operations, query);
        }

        public IBooleanOperation GroupedFlexible(IEnumerable<string> fields, IEnumerable<BooleanOperation> operations, params IExamineValue[] query)
        {
            return this.SearchCriteria.GroupedFlexible(fields, operations, query);
        }

        public IBooleanOperation GroupedNot(IEnumerable<string> fields, params string[] query)
        {
            return this.SearchCriteria.GroupedNot(fields, query);
        }

        public IBooleanOperation GroupedNot(IEnumerable<string> fields, params IExamineValue[] query)
        {
            return this.SearchCriteria.GroupedNot(fields, query);
        }

        public IBooleanOperation GroupedOr(IEnumerable<string> fields, params string[] query)
        {
            return this.SearchCriteria.GroupedOr(fields, query);
        }

        public IBooleanOperation GroupedOr(IEnumerable<string> fields, params IExamineValue[] query)
        {
            return this.SearchCriteria.GroupedOr(fields, query);
        }

        public IBooleanOperation Id(int id)
        {
            return this.SearchCriteria.Id(id);
        }

        public IBooleanOperation NodeName(string nodeName)
        {
            return this.SearchCriteria.NodeName(nodeName);
        }

        public IBooleanOperation NodeName(IExamineValue nodeName)
        {
            return this.SearchCriteria.NodeName(nodeName);
        }

        public IBooleanOperation NodeTypeAlias(string nodeTypeAlias)
        {
            return this.SearchCriteria.NodeTypeAlias(nodeTypeAlias);
        }

        public IBooleanOperation NodeTypeAlias(IExamineValue nodeTypeAlias)
        {
            return this.SearchCriteria.NodeTypeAlias(nodeTypeAlias);
        }

        public IBooleanOperation OrderBy(params string[] fieldNames)
        {
            return this.SearchCriteria.OrderBy(fieldNames);
        }

        public IBooleanOperation OrderByDescending(params string[] fieldNames)
        {
            return this.SearchCriteria.OrderByDescending(fieldNames);
        }

        public IBooleanOperation ParentId(int id)
        {
            return this.SearchCriteria.ParentId(id);
        }

        public IBooleanOperation Range(string fieldName, DateTime lower, DateTime upper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper);
        }

        public IBooleanOperation Range(string fieldName, DateTime lower, DateTime upper, bool includeLower, bool includeUpper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper, includeLower, includeUpper);
        }

        public IBooleanOperation Range(string fieldName, DateTime lower, DateTime upper, bool includeLower, bool includeUpper, DateResolution resolution)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper, includeLower, includeUpper, resolution);
        }

        public IBooleanOperation Range(string fieldName, int lower, int upper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper);
        }

        public IBooleanOperation Range(string fieldName, int lower, int upper, bool includeLower, bool includeUpper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper, includeLower, includeUpper);
        }

        public IBooleanOperation Range(string fieldName, double lower, double upper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper);
        }

        public IBooleanOperation Range(string fieldName, double lower, double upper, bool includeLower, bool includeUpper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper, includeLower, includeUpper);
        }

        public IBooleanOperation Range(string fieldName, float lower, float upper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper);
        }

        public IBooleanOperation Range(string fieldName, float lower, float upper, bool includeLower, bool includeUpper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper, includeLower, includeUpper);
        }

        public IBooleanOperation Range(string fieldName, long lower, long upper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper);
        }

        public IBooleanOperation Range(string fieldName, long lower, long upper, bool includeLower, bool includeUpper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper, includeLower, includeUpper);
        }

        public IBooleanOperation Range(string fieldName, string lower, string upper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper);
        }

        public IBooleanOperation Range(string fieldName, string lower, string upper, bool includeLower, bool includeUpper)
        {
            return this.SearchCriteria.Range(fieldName, lower, upper, includeLower, includeUpper);
        }

        public ISearchCriteria RawQuery(string query)
        {
            return this.SearchCriteria.RawQuery(query);
        }

        #endregion
    }
}
