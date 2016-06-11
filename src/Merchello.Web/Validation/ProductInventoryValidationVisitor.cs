namespace Merchello.Web.Validation
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Used to check for inventory quantities of product items in a line item collection.
    /// </summary>
    public class ProductInventoryValidationVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The list out of stock items.
        /// </summary>
        private readonly List<ILineItem> _outOfStockItems = new List<ILineItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductInventoryValidationVisitor"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The merchello.
        /// </param>
        public ProductInventoryValidationVisitor(MerchelloHelper merchello)
        {
            Mandate.ParameterNotNull(merchello, "merchello");

            _merchello = merchello;
        }

        /// <summary>
        /// Gets the list out of stock items.
        /// </summary>
        public IEnumerable<ILineItem> OutOfStockItems
        {
            get
            {
                return _outOfStockItems;
            }
        } 

        /// <summary>
        /// The visit.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            if (lineItem.LineItemType != LineItemType.Product || !lineItem.AllowsValidation()) return;

            if (!lineItem.ExtendedData.DefinesProductVariant()) return;

            var item = _merchello.Query.Product.GetProductVariantByKey(lineItem.ExtendedData.GetProductVariantKey());

            if (!item.TrackInventory || item.OutOfStockPurchase) return;

            if (item.TotalInventoryCount > 0) return;

            _outOfStockItems.Add(lineItem);
        }
    }
}