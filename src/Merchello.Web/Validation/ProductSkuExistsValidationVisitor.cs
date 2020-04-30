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
    internal class ProductSkuExistsValidationVisitor : ILineItemVisitor
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
        /// Initializes a new instance of the <see cref="ProductSkuExistsValidationVisitor"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The merchello.
        /// </param>
        public ProductSkuExistsValidationVisitor(MerchelloHelper merchello)
        {
            Ensure.ParameterNotNull(merchello, "merchello");

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

            if (!lineItem.ExtendedData.DefinesProductVariant()) return;

            var variant = _merchello.Query.Product.GetProductVariantByKey(lineItem.ExtendedData.GetProductVariantKey());

            if (variant == null) _noLongerExists.Add(lineItem);
        }
    }
}