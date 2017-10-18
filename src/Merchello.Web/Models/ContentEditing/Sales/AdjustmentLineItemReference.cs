namespace Merchello.Web.Models.ContentEditing.Sales
{
    using System;
    using Core;
    using Core.Models;

    /// <summary>
    ///     The adjustment line item reference.
    /// </summary>
    public class AdjustmentLineItemReference
    {
        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Lint item type
        /// </summary>
        public string LineItemType { get; set; }

        /// <summary>
        ///     Gets or sets the Sku
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        ///     Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     Gets or sets the user name of the person who applied the adjustment.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the email the email address of the person who applied the adjustment.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Extended Data
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; }
    }

    /// <summary>
    ///     Mapping extensions for AdjustmentLineItemReference.
    /// </summary>
    internal static class AdjustmentLineItemReferenceExtensions
    {
        /// <summary>
        ///     Maps <see cref="AdjustmentLineItemReference" /> to <see cref="IInvoiceLineItem" />.
        /// </summary>
        /// <param name="adj">
        ///     The adj.
        /// </param>
        /// <returns>
        ///     The <see cref="IInvoiceLineItem" />.
        /// </returns>
        public static IInvoiceLineItem ToInvoiceLineItem(this AdjustmentLineItemReference adj)
        {
            // Default sku
            var sku = Guid.NewGuid().ToString();

            // Get line item type
            var lineItemType = StringToLineItemType(adj.LineItemType);

            if (lineItemType == LineItemType.Product)
            {
                // Product use sku supplied for create new one
                if (!string.IsNullOrEmpty(adj.Sku) && adj.Sku != "adj")
                {
                    sku = adj.Sku;
                }
            }

            // Create invoicelineitem object
            var item = new InvoiceLineItem(lineItemType, adj.Name, sku, 1, adj.Price);

            // See if this adjustment has extended data
            if (adj.ExtendedData != null)
            {
                // If so, add them to the InvoiceLineItem
                foreach (var ed in adj.ExtendedData)
                {
                    item.ExtendedData.SetValue(ed.Key, ed.Value);
                }
            }

            if (adj.Key.Equals(Guid.Empty))
            {
                item.ExtendedData.SetValue("userName", adj.UserName);
                item.ExtendedData.SetValue("email", adj.Email);
                item.ExtendedData.SetValue(Constants.ExtendedDataKeys.Adjustment, adj.LineItemType);
            }
            else
            {
                item.Key = adj.Key;
            }

            return item;
        }

        /// <summary>
        /// Returns the correct lineitem type from a string represention
        /// </summary>
        /// <param name="lineItemType"></param>
        /// <returns></returns>
        private static LineItemType StringToLineItemType(string lineItemType)
        {
            switch (lineItemType)
            {
                case "Product":
                    return LineItemType.Product;
                case "Shipping":
                    return LineItemType.Shipping;
                case "Tax":
                    return LineItemType.Tax;
                default:
                    return LineItemType.Adjustment;
            }
        }
    }
}