namespace Merchello.Web.Validation
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;

    /// <summary>
    /// A visitor to assert product pricing
    /// </summary>
    internal class ProductPricingValidationVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// A list of items that should be removed since the product no longer exists in the back office.
        /// </summary>
        private readonly Dictionary<ILineItem, ProductDisplayBase> _invalidPrices = new Dictionary<ILineItem, ProductDisplayBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductPricingValidationVisitor"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The merchello.
        /// </param>
        public ProductPricingValidationVisitor(MerchelloHelper merchello)
        {
            Mandate.ParameterNotNull(merchello, "merchello");
            _merchello = merchello;
        }

        /// <summary>
        /// Gets the invalid prices.
        /// </summary>
        public IDictionary<ILineItem, ProductDisplayBase> InvalidPrices
        {
            get
            {
                return _invalidPrices;
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

            if (item.OnSale != lineItem.ExtendedData.GetOnSaleValue())
            {
                _invalidPrices.Add(lineItem, item);
                return;
            }

            if ((item.OnSale && (item.SalePrice != lineItem.Price)) || (!item.OnSale && (item.Price != lineItem.Price)))
            {
                _invalidPrices.Add(lineItem, item);
                return;
            }

            // Check if there have been any changes to the product through the service
            if (lineItem.ExtendedData.GetVersionKey() == item.VersionKey) return;

            // on sale
            if (item.OnSale != lineItem.ExtendedData.GetOnSaleValue())
            {
                _invalidPrices.Add(lineItem, item);
            }
        }

    }
}