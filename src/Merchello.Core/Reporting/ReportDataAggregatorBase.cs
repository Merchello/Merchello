namespace Merchello.Core.Reporting
{
    /// <summary>
    /// The report data aggregator base.
    /// </summary>
    public abstract class ReportDataAggregatorBase : IReportDataAggregator
    {
        /// <summary>
        /// Gets or sets the aggregated data.
        /// </summary>
        public abstract object AggregatedData { get; set; }
    }
}