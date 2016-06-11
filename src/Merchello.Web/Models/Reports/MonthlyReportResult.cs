namespace Merchello.Web.Models.Reports
{
    using System;

    /// <summary>
    /// A base class for monthly report results.
    /// </summary>
    public abstract class MonthlyReportResult
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the month name
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public string Year { get; set; }
    }
}