namespace Merchello.Web.Models.SaleHistory
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The daily audit log display.
    /// </summary>
    public class DailyAuditLogDisplay
    {
        /// <summary>
        /// Gets or sets the audit date.
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// Gets or sets the logs.
        /// </summary>
        public IEnumerable<AuditLogDisplay> Logs { get; set; }
    }
}