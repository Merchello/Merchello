namespace Merchello.Web.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    /// <summary>
    /// Visitor to audit basket line item for inventory requirements
    /// </summary>
    public class BasketItemInventoryValidation : ILineItemVisitor
    {
        #region Fields

        /// <summary>
        /// The <see cref="IProductVariantService"/>.
        /// </summary>
        private readonly IProductVariantService _productVariantService;

        /// <summary>
        /// A collection of items that failed the inventory validation check.
        /// </summary>
        private readonly List<InventoryValidation> _failedInventoryValidation;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketItemInventoryValidation"/> class.
        /// </summary>
        /// <param name="productVariantService">
        /// The <see cref="ProductVariantService"/>.
        /// </param>
        public BasketItemInventoryValidation(IProductVariantService productVariantService)
        {
            Mandate.ParameterNotNull(productVariantService, "productVariantService");

            _productVariantService = productVariantService;
            this._failedInventoryValidation = new List<InventoryValidation>();
        }

        /// <summary>
        /// Gets the failed inventory validations.
        /// </summary>
        public IEnumerable<InventoryValidation> FailedInventoryValidations
        {
            get
            {
                return this._failedInventoryValidation;
            }
        }

        /// <summary>
        /// Gets a value indicating whether passes inventory requirements.
        /// </summary>
        public bool PassesInventoryRequirements
        {
            get { return !this._failedInventoryValidation.Any(); }
        }

        /// <summary>
        /// Visits the line item
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            // if the line item does not have a product variant reference this vistor cannot check it 
            if (!lineItem.ExtendedData.ContainsProductVariantKey()) return;           

            // if the variants inventory is not tracked or if out of stock purchases are allowed check is not necessary
            if (!lineItem.ExtendedData.GetTrackInventoryValue() || lineItem.ExtendedData.GetOutOfStockPurchaseValue()) return;

            var variant = _productVariantService.GetByKey(lineItem.ExtendedData.GetProductVariantKey());

            if (variant == null) return;

            if (variant.TotalInventoryCount < lineItem.Quantity)
            {
                this._failedInventoryValidation.Add(new InventoryValidation(lineItem, variant.TotalInventoryCount));
            }
        }

        /// <summary>
        /// The inventory validation.
        /// </summary>
        public class InventoryValidation
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InventoryValidation"/> class.
            /// </summary>
            /// <param name="requested">
            /// The requested.
            /// </param>
            /// <param name="inventoryCount">
            /// The inventory count.
            /// </param>
            public InventoryValidation(ILineItem requested, int inventoryCount)
            {
                Requested = requested;
                InventoryCount = inventoryCount;
            }

            /// <summary>
            /// Gets the requested amount
            /// </summary>
            public ILineItem Requested { get; private set; }

            /// <summary>
            /// Gets the inventory count.
            /// </summary>
            public int InventoryCount { get; private set; }
        }
    }
}