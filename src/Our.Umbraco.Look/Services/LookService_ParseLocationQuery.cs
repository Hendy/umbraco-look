using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Spatial.Tier;
using Lucene.Net.Util;
using Our.Umbraco.Look.Models;
using System;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookQuery">The query to parse</param>
        /// <param name="parsingContext"></param>
        private static void ParseLocationQuery(LookQuery lookQuery, ParsingContext parsingContext)
        {
            if (lookQuery.LocationQuery != null)
            {
                parsingContext.QueryAdd(new TermQuery(new Term(LookConstants.HasLocationField, "1")), BooleanClause.Occur.MUST);

                if (lookQuery.LocationQuery.Boundary != null) // limit results within an lat lng fixed view (eg, typical map bounds)
                {
                    parsingContext.QueryAdd(
                            new TermRangeQuery(
                                LookConstants.LocationField + "_Latitude",
                                NumericUtils.DoubleToPrefixCoded(lookQuery.LocationQuery.Boundary.LatitudeMin),
                                NumericUtils.DoubleToPrefixCoded(lookQuery.LocationQuery.Boundary.LatitudeMax),
                                true,
                                true),
                            BooleanClause.Occur.MUST);

                    parsingContext.QueryAdd(
                            new TermRangeQuery(
                                LookConstants.LocationField + "_Longitude",
                                NumericUtils.DoubleToPrefixCoded(lookQuery.LocationQuery.Boundary.LongitudeMin),
                                NumericUtils.DoubleToPrefixCoded(lookQuery.LocationQuery.Boundary.LongitudeMax),
                                true,
                                true),
                            BooleanClause.Occur.MUST);
                }

                if (lookQuery.LocationQuery.Location != null) // location set, so can calculate distance
                {
                    double maxDistance = LookService._maxDistance;

                    if (lookQuery.LocationQuery.MaxDistance != null)
                    {
                        maxDistance = Math.Min(lookQuery.LocationQuery.MaxDistance.GetMiles(), maxDistance);
                    }

                    var distanceQueryBuilder = new DistanceQueryBuilder(
                                                lookQuery.LocationQuery.Location.Latitude,
                                                lookQuery.LocationQuery.Location.Longitude,
                                                maxDistance,
                                                LookConstants.LocationField + "_Latitude",
                                                LookConstants.LocationField + "_Longitude",
                                                LookConstants.LocationTierFieldPrefix,
                                                true);

                    parsingContext.Filter = distanceQueryBuilder.Filter;

                    if (lookQuery.SortOn == SortOn.Distance)
                    {
                        parsingContext.Sort = new Sort(
                                                new SortField(
                                                    LookConstants.DistanceField,
                                                    new DistanceFieldComparatorSource(distanceQueryBuilder.DistanceFilter)));
                    }

                    parsingContext.GetDistance = new Func<int, double?>(x =>
                    {
                        if (distanceQueryBuilder.DistanceFilter.Distances.ContainsKey(x))
                        {
                            return distanceQueryBuilder.DistanceFilter.Distances[x];
                        }

                        return null;
                    });
                }
            }
        }
    }
}
