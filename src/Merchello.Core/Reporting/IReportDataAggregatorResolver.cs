namespace Merchello.Core.Reporting
{
    using System;

    /// <summary>
    /// Defines the ReportDataAggregatorResolver.
    /// </summary>
    public interface IReportDataAggregatorResolver
    {
        /// <summary>
        /// Gets a report data aggregator by it's key defined in the attribute
        /// </summary>
        /// <param name="aggregatorKey">
        /// The aggregator key.
        /// </param>
        /// <returns>
        /// The <see cref="IReportDataAggregator"/>.
        /// </returns>
        IReportDataAggregator GetByKey(Guid aggregatorKey);
    }
}