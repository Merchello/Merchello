namespace Merchello.Core.Reporting
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Marker interface for Report Data Aggregators
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result to return
    /// </typeparam>
    /// <typeparam name="TQuerySettings">
    /// Object that contains compilation (query) parameters
    /// </typeparam>
    /// <remarks>
    /// Report data aggregators are used by the ReportApiController (in .Web) to collect data for reports
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public interface IReportDataAggregatorBase<out TResult, in TQuerySettings>
    {
        /// <summary>
        /// Compiles the report data.
        /// </summary>
        /// <param name="querySettings">
        /// The query Settings.
        /// </param>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        TResult GetReportData(TQuerySettings querySettings);
    }
}