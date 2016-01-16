namespace Merchello.Web.Editors.Reports
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Models.Reports;
    using Merchello.Web.Reporting;

    /// <summary>
    /// The monthly report api controller base.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result to be returned by the API
    /// </typeparam>
    public abstract class MonthlyReportApiControllerBase<TResult> : ReportController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyReportApiControllerBase{TResult}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected MonthlyReportApiControllerBase(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        /// <summary>
        /// Builds the report data for a custom date range
        /// </summary>
        /// <param name="query">A <see cref="QueryResultDisplay"/> containing the date range</param>
        /// <returns>The <see cref="QueryResultDisplay"/></returns>
        [HttpPost]
        public virtual QueryResultDisplay GetCustomDateRange(QueryDisplay query)
        {
            var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
            var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");

            var isDateSearch = invoiceDateStart != null && !string.IsNullOrEmpty(invoiceDateStart.Value) &&
               invoiceDateEnd != null && !string.IsNullOrEmpty(invoiceDateEnd.Value);

            if (!isDateSearch) return GetDefaultReportData();

            DateTime startDate;
            //// Assert the start date
            if (DateTime.TryParse(invoiceDateStart.Value, out startDate))
            {
                DateTime endDate;
                //// Assert the end date
                if (DateTime.TryParse(invoiceDateEnd.Value, out endDate))
                {
                    //// Return the default report if startDate >= endDate
                    if (startDate >= endDate) return GetDefaultReportData();

                    var endOfMonth = GetEndOfMonth(endDate);
                    var startOfYear = GetFirstOfMonth(startDate);

                    return BuildResult(startOfYear, endOfMonth);

                }

                return GetDefaultReportData();
            }

            return GetDefaultReportData();
        }

        /// <summary>
        /// Gets the first day a month month.
        /// </summary>
        /// <param name="current">
        /// The reference date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        protected static DateTime GetFirstOfMonth(DateTime current)
        {
            return current.FirstOfMonth();
        }

        /// <summary>
        /// Gets the last day of a month.
        /// </summary>
        /// <param name="current">
        /// The reference date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        protected static DateTime GetEndOfMonth(DateTime current)
        {
            return current.EndOfMonth();
        }

        /// <summary>
        /// Builds the result set for the report.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        protected abstract QueryResultDisplay BuildResult(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Performs the actual work of querying for the results.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The typed result.
        /// </returns>
        protected abstract TResult GetResults(DateTime startDate, DateTime endDate);
    }
}