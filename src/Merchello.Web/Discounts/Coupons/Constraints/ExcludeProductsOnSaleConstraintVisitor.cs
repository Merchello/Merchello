namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The product selection constraint visitor.
    /// </summary>
    internal class ExcludeProductsOnSaleConstraintVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The filtered items.
        /// </summary>
        private readonly List<ILineItem> _filteredItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeProductsOnSaleConstraintVisitor"/> class.
        /// </summary>
        public ExcludeProductsOnSaleConstraintVisitor()
        {
            _filteredItems = new List<ILineItem>();
        }

        /// <summary>
        /// Gets the filtered items.
        /// </summary>
        public IEnumerable<ILineItem> FilteredItems
        {
            get
            {
                return _filteredItems;
            }
        }

        /// <summary>
        /// Visits the line items to apply expression.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param> 
        public void Visit(ILineItem lineItem)
        {
            if (lineItem.LineItemType != LineItemType.Product)
            {
                _filteredItems.Add(lineItem);
                return;
            }

            // Check product is on sale
            var productKey = lineItem.ExtendedData.GetProductKey();
            if (productKey.Equals(Guid.Empty)) return;
            var product = MerchelloContext.Current.Services.ProductService.GetByKey(productKey);
            if (product.OnSale)
            {
                return;
            }

            // Check product variant is on sale
            var productVariantKey = lineItem.ExtendedData.GetProductVariantKey();
            if (productVariantKey.Equals(Guid.Empty)) return;
            var productVariant = MerchelloContext.Current.Services.ProductVariantService.GetByKey(productVariantKey);
            if (productVariant.OnSale)
            {
                return;
            }

            _filteredItems.Add(lineItem);
        }
    }
}