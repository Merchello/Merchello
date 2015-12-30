namespace Merchello.Web.Models.Reports
{
    /// <summary>
    /// The sales over time result.
    /// </summary>
    public class SalesOverTimeResult
    {
        /// <summary>
        /// Gets or sets the date string
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        ///  Gets or sets the total amount of sales
        /// </summary>
        public decimal SalesTotal { get; set; }

        /// <summary>
        ///  Gets or sets the number of sales
        /// </summary>
        public int SalesCount { get; set; }
    }
}