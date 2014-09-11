namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    using System;
    using System.Collections.Generic;

    using Merchello.Plugin.Taxation.Avalara.Models.Address;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a "Get" tax request.
    /// </summary>
    public class TaxRequest : TaxRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxRequest"/> class.
        /// </summary>
        /// <param name="docType">
        /// The doc Type. This determines if the quotation should be recorded or if it is just an estimate.
        /// Defaults to an estimate.
        /// </param>
        public TaxRequest(StatementType docType = StatementType.SalesOrder)
        {
            DocDate = DateTime.Today.ToString("yyyy-M-dddd");
            DetailLevel = DetailLevel.Tax;
            Commit = false;
            DocType = docType;
        }

        // Required for tax calculation

        /// <summary>
        /// Gets or sets the doc date.  Example Invoice date
        /// </summary>
        public string DocDate { get; set; }

        /// <summary>
        /// Gets or sets the customer code.
        /// </summary>
        /// <remarks>
        /// This is the 
        /// </remarks>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Gets or sets the addresses associated with the tax request
        /// </summary>
        public IEnumerable<TaxAddress> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        public IEnumerable<StatementLineItem> Lines { get; set; }

        //// Best Practice for tax calculation

        /// <summary>
        /// Gets the client software name
        /// </summary>
        public string Client 
        {
            get
            {
                return "Merchello Avalara AvaTax Plugin";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to commit the request.
        /// Default is false. Setting this value to true will prevent further document status changes, except voiding with CancelTax.
        /// </summary>
        public bool Commit { get; set; }

        /// <summary>
        /// Gets or sets the detail level which specifies the level of detail to return.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DetailLevel DetailLevel { get; set; }

        //// Use where appropriate to the situation

        /// <summary>
        /// Gets or sets the customer usage type.
        /// The client application customer or usage type.
        /// </summary>
        public string CustomerUsageType { get; set; }

        /// <summary>
        /// Gets or sets the exemption no.
        /// </summary>
        /// <remarks>
        /// Any string value will cause the sale to be exempt. This should only be used if your finance team is manually verifying and tracking exemption certificates.
        /// </remarks>
        public string ExemptionNo { get; set; }

        /// <summary>
        /// Gets or sets the discount applied to the statement
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets the business identification number.  Really only required if you are concerned with VAT
        /// </summary>
        public string BusinessIdentificationNo { get; set; }

        /// <summary>
        /// Gets or sets the tax override rules for the statement
        /// </summary>
        public TaxOverride TaxOverride { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        // Optional

        /// <summary>
        /// Gets or sets the purchase order number.
        /// </summary>
        public string PurchaseOrderNo { get; set; }

        /// <summary>
        /// Gets or sets the payment date.
        /// </summary>
        public string PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the Point of Sale lane code.  Useful in point of sale applications.
        /// </summary>
        public string PosLaneCode { get; set; }

        /// <summary>
        /// Gets or sets the reference code.
        /// Used for returns and refers to the DocCode of the original invoice.
        /// </summary>
        public string ReferenceCode { get; set; }
    }
}