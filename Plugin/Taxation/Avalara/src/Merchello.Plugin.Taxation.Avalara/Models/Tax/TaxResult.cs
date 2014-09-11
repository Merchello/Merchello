namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    using System;
    using System.Collections.Generic;
    using Address;

    /// <summary>
    /// The tax result.
    /// </summary>
    public class TaxResult
    {
        /// <summary>
        /// Gets the extended data key.
        /// </summary>
        public static string ExtendedDataKey
        {
            get
            {
                return "merchAvaTaxTaxResult";
            }
        }

        /// <summary>
        /// Gets or sets the doc code.
        /// </summary>
        public string DocCode { get; set; }

        /// <summary>
        /// Gets or sets the doc date.
        /// </summary>
        public DateTime DocDate { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the total discount.
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// Gets or sets the total exemption.
        /// </summary>
        public decimal TotalExemption { get; set; }

        /// <summary>
        /// Gets or sets the total taxable.
        /// </summary>
        public decimal TotalTaxable { get; set; }

        /// <summary>
        /// Gets or sets the total tax.
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Gets or sets the total tax calculated.
        /// </summary>
        public decimal TotalTaxCalculated { get; set; }

        /// <summary>
        /// Gets or sets the tax date.
        /// </summary>
        public DateTime TaxDate { get; set; }

        /// <summary>
        /// Gets or sets the tax lines.
        /// </summary>
        public IEnumerable<StatementLineItem> TaxLines { get; set; }

        /// <summary>
        /// Gets or sets the tax summary.
        /// </summary>
        public IEnumerable<StatementLineItem> TaxSummary { get; set; }

        /// <summary>
        /// Gets or sets the tax addresses.
        /// </summary>
        public IEnumerable<TaxAddress> TaxAddresses { get; set; }

        /// <summary>
        /// Gets or sets the result code.
        /// </summary>
        public SeverityLevel ResultCode { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="IApiResponseMessage"/> returned from the API.
        /// </summary>
        public IEnumerable<ApiResponseMessage> Messages { get; set; }
    }
}