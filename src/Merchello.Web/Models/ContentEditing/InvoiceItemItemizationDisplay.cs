namespace Merchello.Web.Models.ContentEditing
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Strategies.Itemization;

    /// <summary>
    /// Represent an invoice item itemization.
    /// </summary>
    public class InvoiceItemItemizationDisplay
    {
        /// <summary>
        /// Gets or sets the collection of the product line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Products { get; set; }

        /// <summary>
        /// Gets or sets the collection of shipping line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Shipping { get; set; }

        /// <summary>
        /// Gets or sets the collection of tax line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Tax { get; set; }

        /// <summary>
        /// Gets or sets the collection of adjustment line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Adjustments { get; set; }

        /// <summary>
        /// Gets or sets the collection of discount line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Discounts { get; set; }

        /// <summary>
        /// Gets or sets the collection of custom line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Custom { get; set; }

        /// <summary>
        /// Gets a value indicating whether reconciles.
        /// </summary>
        public bool Reconciles { get; internal set; }


        /// <summary>
        /// Gets the shipping total.
        /// </summary>
        public decimal ShippingTotal { get; internal set; }

        /// <summary>
        /// Gets the tax total.
        /// </summary>
        public decimal TaxTotal { get; internal set; }

        /// <summary>
        /// Gets the adjustment total.
        /// </summary>
        public decimal AdjustmentTotal { get; internal set; }

        /// <summary>
        /// Gets the product total.
        /// </summary>
        public decimal ProductTotal { get; internal set; }

        /// <summary>
        /// Gets the discount total.
        /// </summary>
        public decimal DiscountTotal { get; internal set; }

        /// <summary>
        /// Gets the custom total.
        /// </summary>
        public decimal CustomTotal { get; internal set; }

        /// <summary>
        /// Gets the invoice total of the invoice.
        /// </summary>
        public decimal InvoiceTotal { get; internal set; }

        /// <summary>
        /// Gets the total of the invoice itemization.
        /// </summary>
        public decimal ItemizationTotal { get; internal set; }
    }

    /// <summary>
    /// Extension methods for <see cref="InvoiceItemItemizationDisplay"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class InvoiceItemItemizationDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="InvoiceItemItemization"/> to <see cref="InvoiceItemItemizationDisplay"/>.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceItemItemizationDisplay"/>.
        /// </returns>
        public static InvoiceItemItemizationDisplay ToInvoiceItemItemizationDisplay(this InvoiceItemItemization itemization)
        {
            return AutoMapper.Mapper.Map<InvoiceItemItemizationDisplay>(itemization);
        }
    }
}