namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="IItemCache"/>
    /// </summary>
    public static class LineItemContainerExtensions
    {

        #region AddItem

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, IProductVariant productVariant, int quantity)
        {
            var extendedData = new ExtendedDataCollection();
            extendedData.AddProductVariantValues(productVariant);

            container.AddItem(LineItemType.Product, productVariant.Name, productVariant.Sku, quantity, productVariant.Price, extendedData);
        }

        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, LineItemType lineItemType, string name, string sku, int quantity, decimal amount)
        {
            container.AddItem(lineItemType, name, sku, quantity, amount, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData)
        {
            container.AddItem(new ItemCacheLineItem(container.Key, lineItemType, name, sku, quantity, amount, extendedData));
        }

        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, IItemCacheLineItem lineItem)
        {
            container.Items.Add(lineItem);
        }

        #endregion

    }
}

