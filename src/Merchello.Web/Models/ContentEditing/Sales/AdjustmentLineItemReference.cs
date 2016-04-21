namespace Merchello.Web.Models.ContentEditing.Sales
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The adjustment line item reference.
    /// </summary>
    public class AdjustmentLineItemReference
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the user name of the person who applied the adjustment.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email the email address of the person who applied the adjustment.
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// Mapping extensions for AdjustmentLineItemReference.
    /// </summary>
    internal static class AdjustmentLineItemReferenceExtensions
    {
        /// <summary>
        /// Maps <see cref="AdjustmentLineItemReference"/> to <see cref="IInvoiceLineItem"/>.
        /// </summary>
        /// <param name="adj">
        /// The adj.
        /// </param>
        /// <returns>
        /// The <see cref="IInvoiceLineItem"/>.
        /// </returns>
        public static IInvoiceLineItem ToInvoiceLineItem(this AdjustmentLineItemReference adj)
        {
            var item = new InvoiceLineItem(LineItemType.Adjustment, adj.Name, Guid.NewGuid().ToString(), 1, adj.Price);
            if (adj.Key.Equals(Guid.Empty))
            {
                item.ExtendedData.SetValue("userName", adj.UserName);
                item.ExtendedData.SetValue("email", adj.Email);
            }
            else
            {
                item.Key = adj.Key;
            }

            return item;
        }
    }
}