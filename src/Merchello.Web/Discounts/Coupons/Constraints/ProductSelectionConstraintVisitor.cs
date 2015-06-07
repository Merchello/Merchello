namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Models;

    /// <summary>
    /// The product selection constraint visitor.
    /// </summary>
    internal class ProductSelectionConstraintVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The product constraints.
        /// </summary>
        private readonly ProductConstraintData[] _constraints;

        /// <summary>
        /// The filtered items.
        /// </summary>
        private readonly List<ILineItem> _filteredItems; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSelectionConstraintVisitor"/> class.
        /// </summary>
        /// <param name="productContraints">
        /// The product constraints.
        /// </param>
        public ProductSelectionConstraintVisitor(IEnumerable<ProductConstraintData> productContraints)
        {
            _constraints = productContraints.ToArray();
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

            // gets the product key from the line item extended data
            var productKey = lineItem.ExtendedData.GetProductKey();

            if (productKey.Equals(Guid.Empty)) return;

            // look for a matching value in the constraints collection
            var found = _constraints.FirstOrDefault(x => x.ProductKey == productKey);
            if (found == null) return;

            // if this constraint does not specified variants add it to the filtered collection
            if (!found.SpecifiedVariants)
            {
                _filteredItems.Add(lineItem);
                return;
            }

            // check the product variant key
            var productVariantKey = lineItem.ExtendedData.GetProductVariantKey();

            if (productVariantKey.Equals(Guid.Empty)) return;

            var foundVariant = found.VariantKeys.FirstOrDefault(x => x == productVariantKey);

            // add the line item to the collection if the variant key is found
            if (!foundVariant.Equals(Guid.Empty)) _filteredItems.Add(lineItem);
        }
    }
}