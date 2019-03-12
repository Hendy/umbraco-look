using Our.Umbraco.Look.Extensions;
using System;
using System.Globalization;
using Umbraco.Core.Models;

namespace Our.Umbraco.Look
{
    /// <summary>
    /// All properies are optional, when set clauses are ANDed
    /// </summary>
    public class NodeQuery
    {
        /// <summary>
        /// If set, then items returned must be of this type (will take precedence over TypeAny)
        /// </summary>
        public PublishedItemType? Type { get; set; } = null;

        /// <summary>
        /// If set, then items returned can be any of these types
        /// </summary>
        public PublishedItemType[] TypeAny { get; set; } = null;

        /// <summary>
        /// Flags to indicate whether detached content should be returned
        /// </summary>
        public DetachedQuery DetachedQuery { get; set; } = DetachedQuery.IncludeDetached;

        /// <summary>
        /// If set, then items returned must be of this culture (will take precedence over CultureAny)
        /// </summary>
        public CultureInfo Culture { get; set; } = null;

        /// <summary>
        /// The cultures of the content nodes to find
        /// </summary>
        public CultureInfo[] CultureAny { get; set; } = null;

        /// <summary>
        /// If set, then items returned must be of this alias (will take precedence over AliasAny)
        /// </summary>
        public string Alias { get; set; } = null;

        /// <summary>
        /// The document type, media type or member type aliases
        /// </summary>
        public string[] AliasAny { get; set; } = null;

        /// <summary>
        /// Only content, media or members with these ids will be retuned (detached items don't have ids)
        /// </summary>
        public int[] Ids { get; set; }

        /// <summary>
        /// If not null, then each result returned must have a key in this collection (useful for finding detached content)
        /// </summary>
        public Guid[] Keys { get; set; } = null;

        /// <summary>
        /// Results must not have this id
        /// </summary>
        public int? NotId { get; set; } = null;

        /// <summary>
        /// Any umbraco ids that should be exlcuded from the results
        /// </summary>
        public int[] NotIds { get; set; } = null;

        /// <summary>
        /// Results must not have this key
        /// </summary>
        public Guid? NotKey { get; set; } = null;

        /// <summary>
        /// Any keys that should be excluded from the results
        /// </summary>
        public Guid[] NotKeys { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            NodeQuery nodeQuery = obj as NodeQuery;

            return nodeQuery != null
                && nodeQuery.Type == this.Type
                && nodeQuery.TypeAny.BothNullOrElementsEqual(this.TypeAny)
                && nodeQuery.DetachedQuery == this.DetachedQuery
                && nodeQuery.Culture == this.Culture
                && nodeQuery.CultureAny.BothNullOrElementsEqual(this.CultureAny)
                && nodeQuery.Alias == this.Alias
                && nodeQuery.AliasAny.BothNullOrElementsEqual(this.AliasAny)
                && nodeQuery.Ids.BothNullOrElementsEqual(this.Ids)
                && nodeQuery.Keys.BothNullOrElementsEqual(this.Keys)
                && nodeQuery.NotId == this.NotId
                && nodeQuery.NotIds.BothNullOrElementsEqual(this.NotIds)
                && nodeQuery.NotKey == this.NotKey
                && nodeQuery.NotKeys.BothNullOrElementsEqual(this.Keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal NodeQuery Clone()
        {
            return (NodeQuery)this.MemberwiseClone();
        }
    }
}
