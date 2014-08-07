namespace Merchello.Core.Reporting
{
    /// <summary>
    /// Defines a Report Data Aggregator
    /// </summary>
    /// <remarks>
    /// Report data aggregators are used by the ReportApiController (in .Web) to collect data for reports
    /// </remarks>
    public interface IReportDataAggregator
    {
        /// <summary>
        /// Responsible for compiling the report data.
        /// </summary>
        void Compile();
    }
}