namespace Merchello.Web.Validation
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;

    /// <summary>
    /// A visitor to check if products still exist in Merchello.
    /// </summary>
    internal class ProductSkuExistsVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// A list of items that should be removed since the product no longer exists in the back office.
        /// </summary>
        private readonly List<ILineItem> _noLongerExists = new List<ILineItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSkuExistsVisitor"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The merchello.
        /// </param>
        public ProductSkuExistsVisitor(MerchelloHelper merchello)
        {
            Mandate.ParameterNotNull(merchello, "merchello");

            _merchello = merchello;
        }

        /// <summary>
        /// Gets the line items to remove.
        /// </summary>
        public IEnumerable<ILineItem> LineItemsToRemove
        {
            get
            {
                return _noLongerExists;
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

            var isVariant = lineItem.ExtendedData.DefinesProductVariant();

            var item = isVariant ? 
                (ProductDisplayBase)_merchello.Query.Product.GetProductVariantBySku(lineItem.Sku) : 
                _merchello.Query.Product.GetBySku(lineItem.Sku);

            if (item == null) _noLongerExists.Add(lineItem);
        }
    }
}