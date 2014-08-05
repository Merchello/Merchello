namespace Merchello.Core.Reporting
{
    /// <summary>
    /// Defines a Report Data Aggregator
    /// </summary>
    public interface IReportDataAggregator
    {
        /// <summary>
        /// Responsible for compiling the report data.
        /// </summary>
        void Compile();
    }
}