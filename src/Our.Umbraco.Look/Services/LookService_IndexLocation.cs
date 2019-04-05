using Lucene.Net.Documents;
using Lucene.Net.Util;
using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Look.Services
{
    internal partial class LookService
    {
        private static void IndexLocation(IndexingContext indexingContext, Document document)
        {
            if (indexingContext.Cancelled) return;

            if (LookService.GetLocationIndexer() != null)
            {
                Location location = null;

                try
                {
                    location = LookService.GetLocationIndexer()(indexingContext);
                }
                catch (Exception exception)
                {
                    LogHelper.WarnWithException(typeof(LookService), "Error in location indexer", exception);
                }

                if (location != null)
                {
                    var hasLocationField = new Field(
                                                LookConstants.HasLocationField,
                                                "1",
                                                Field.Store.NO,
                                                Field.Index.NOT_ANALYZED);

                    var locationField = new Field(
                                                LookConstants.LocationField,
                                                location.ToString(),
                                                Field.Store.YES,
                                                Field.Index.NOT_ANALYZED);

                    var locationLatitudeField = new Field(
                                            LookConstants.LocationField + "_Latitude",
                                            NumericUtils.DoubleToPrefixCoded(location.Latitude),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED);

                    var locationLongitudeField = new Field(
                                        LookConstants.LocationField + "_Longitude",
                                        NumericUtils.DoubleToPrefixCoded(location.Longitude),
                                        Field.Store.YES,
                                        Field.Index.NOT_ANALYZED);

                    document.Add(hasLocationField);
                    document.Add(locationField);
                    document.Add(locationLatitudeField);
                    document.Add(locationLongitudeField);

                    foreach (var cartesianTierPlotter in LookService.Instance._cartesianTierPlotters)
                    {
                        var boxId = cartesianTierPlotter.GetTierBoxId(location.Latitude, location.Longitude);

                        var tierField = new Field(
                                            cartesianTierPlotter.GetTierFieldName(),
                                            NumericUtils.DoubleToPrefixCoded(boxId),
                                            Field.Store.YES,
                                            Field.Index.NOT_ANALYZED_NO_NORMS);

                        document.Add(tierField);
                    }
                }
            }
        }
    }
}
