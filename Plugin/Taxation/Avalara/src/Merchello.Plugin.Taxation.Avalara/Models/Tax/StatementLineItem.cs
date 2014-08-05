namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    /// <summary>
    /// Represents a statement line item used in a Tax Request
    /// </summary>
    public class StatementLineItem
    {
        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <remarks>
        /// Required field
        /// </remarks>
        public string LineNo { get; set; }

        /// <summary>
        /// Gets or sets the destination code.
        /// </summary>
        /// <remarks>
        /// Required field
        /// </remarks>
        public string DestinationCode { get; set; }

        /// <summary>
        /// Gets or sets the origin code.
        /// </summary>
        /// <remarks>
        /// Required field
        /// </remarks>
        public string OriginCode { get; set; }

        /// <summary>
        /// Gets or sets the item code.
        /// </summary>
        /// <remarks>
        /// Required field
        /// </remarks>
        public string ItemCode { get; set; } // Required

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <remarks>
        /// Required field
        /// </remarks>
        public decimal Qty { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// This is the total line item amount - quantity * price
        /// </summary>
        /// <remarks>
        /// Required field
        /// </remarks>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the tax code.
        /// </summary>
        /// <remarks>Best practice</remarks>
        public string TaxCode { get; set; }

        /// <summary>
        /// Gets or sets the customer usage type.
        /// </summary>
        public string CustomerUsageType { get; set; }

        /// <summary>
        /// Gets or sets the tax override rules to be applied to the line.
        /// </summary>
        public TaxOverride TaxOverride { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <remarks>
        /// Best practice.  This should be the line item name
        /// </remarks>
        public string Description { get; set; } // Best Practice

        /// <summary>
        /// Gets or sets a value indicating whether the line item has been discounted.
        /// </summary>
        public bool Discounted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tax is already included.
        /// </summary>
        public bool TaxIncluded { get; set; }

        /// <summary>
        /// Gets or sets a value that refers to the DocCode of the original invoice.
        /// </summary>
        public string Ref1 { get; set; }

        /// <summary>
        /// Gets or sets a value that refers to the DocCode of the original invoice.
        /// </summary>
        public string Ref2 { get; set; } 
    }
}