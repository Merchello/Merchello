namespace Merchello.Core.Reporting
{
    /// <summary>
    /// The report data aggregator base.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result to be returned
    /// </typeparam>
    /// <typeparam name="TQuerySettings">
    /// Object that contains compilation (query) parameters
    /// </typeparam>
    public abstract class ReportDataAggregatorBase<TResult, TQuerySettings> : IReportDataAggregatorBase<TResult, TQuerySettings>
    {
        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="querySettings">
        /// The query settings.
        /// </param>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        public abstract TResult GetReportData(TQuerySettings querySettings);
    }
}