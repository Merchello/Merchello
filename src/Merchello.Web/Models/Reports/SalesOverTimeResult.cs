namespace Merchello.Web.Models.Reports
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The sales over time result.
    /// </summary>
    public class SalesOverTimeResult : MonthlyReportResult
    {
        /// <summary>
        ///  Gets or sets the number of sales
        /// </summary>
        public int SalesCount { get; set; }

        /// <summary>
        /// Gets or sets the totals.
        /// </summary>
        public IEnumerable<ResultCurrencyValue> Totals { get; set; }
    }
}