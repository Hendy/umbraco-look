using Examine.SearchCriteria;
using System;
using System.Collections.Generic;

namespace Our.Umbraco.Look
{
    public class LookSearchCriteria : ISearchCriteria
    {
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
    }
}
