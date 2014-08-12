namespace Merchello.Web.Reporting.Sales
{
    using System;

    /// <summary>
    /// The sales over time query settings.
    /// </summary>
    public class SalesOverTimeQuerySettings
    {
        /// <summary>
        /// Gets or sets the begin date time.
        /// </summary>
        public DateTime BeginDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date time.
        /// </summary>
        public DateTime EndDateTime { get; set; }
    }
}