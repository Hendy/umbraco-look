using System;

namespace Our.Umbraco.Look.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DateQuery
    {
        /// <summary>
        /// If set, only results before this date/time are returned
        /// </summary>
        public DateTime? Before { get; set; } = null;

        /// <summary>
        /// If set, only results after this date/time are returned
        /// </summary>
        public DateTime? After { get; set; } = null;

        /// <summary>
        /// Enum flag to specify which date clauses are inclusive or exclusive
        /// </summary>
        public DateBoundary Boundary { get; set; } = DateBoundary.Inclusive;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="before"></param>
        ///// <param name="after"></param>
        //public DateQuery(DateTime? before = null, DateTime? after = null)
        //{
        //    this.Before = before;
        //    this.After = after;
        //}

        public override bool Equals(object obj)
        {
            var dateQuery = obj as DateQuery;

            return dateQuery != null
                && dateQuery.After == this.After
                && dateQuery.Before == this.Before;
        }

        internal DateQuery Clone()
        {
            return (DateQuery)this.MemberwiseClone();
        }
    }
}
