namespace Merchello.Web.Models.SaleHistory
{
    using System.Collections.Generic;

    /// <summary>
    /// The sales history display.
    /// </summary>
    public class SalesHistoryDisplay
    {
        /// <summary>
        /// Gets or sets the daily logs.
        /// </summary>
        public IEnumerable<DailyAuditLogDisplay> DailyLogs { get; set; }
    }
}