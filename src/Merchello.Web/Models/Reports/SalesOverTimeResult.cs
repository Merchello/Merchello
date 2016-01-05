namespace Merchello.Web.Models.Reports
{
    using System.Collections.Generic;

    /// <summary>
    /// The sales over time result.
    /// </summary>
    public class SalesOverTimeResult
    {
        /// <summary>
        /// Gets or sets the month name
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public string Year { get; set; }

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