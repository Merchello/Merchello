namespace Merchello.Web.Models.Reports
{
    using System;

    /// <summary>
    /// The abandoned basket result.
    /// </summary>
    internal class AbandonedBasketResult
    {
        /// <summary>
        /// Gets or sets the configured days.
        /// </summary>
        public int ConfiguredDays { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the anonymous basket count.
        /// </summary>
        public int AnonymousBasketCount { get; set; }

        /// <summary>
        /// Gets or sets the anonymous checkout count.
        /// </summary>
        public int AnonymousCheckoutCount { get; set; }

        /// <summary>
        /// Gets or sets the anonymous checkout percent.
        /// </summary>
        public decimal AnonymousCheckoutPercent { get; set; }

        /// <summary>
        /// Gets or sets the customer basket count.
        /// </summary>
        public int CustomerBasketCount { get; set; }

        /// <summary>
        /// Gets or sets the customer checkout count.
        /// </summary>
        public int CustomerCheckoutCount { get; set; }

        /// <summary>
        /// Gets or sets the customer checkout percent.
        /// </summary>
        public decimal CustomerCheckoutPercent { get; set; }
    }
}