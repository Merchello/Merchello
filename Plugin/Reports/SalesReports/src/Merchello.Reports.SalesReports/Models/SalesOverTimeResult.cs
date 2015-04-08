
namespace Merchello.Reports.SalesReports.Models
{
    using System;

    /// <summary>
    /// The sales over time result.
    /// </summary>
    public class SalesOverTimeResult
    {
        /// <summary>
        /// date string
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Total amount of sales
        /// </summary>
        public Decimal SalesTotal { get; set; }

        /// <summary>
        /// Number of sales
        /// </summary>
        public int SalesCount { get; set; }
    }
}