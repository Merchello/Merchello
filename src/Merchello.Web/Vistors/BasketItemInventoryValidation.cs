using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Web.Vistors
{
    /// <summary>
    /// Visitor to audit basket line item for inventory requirements
    /// </summary>
    public class BasketItemInventoryValidation : ILineItemVisitor
    {

        private readonly IProductVariantService _productVariantService;
        private readonly List<InventoryValidation> _failedInventoryValidtion;


        public BasketItemInventoryValidation(IProductVariantService productVariantService)
        {
            Mandate.ParameterNotNull(productVariantService, "productVariantService");

            _productVariantService = productVariantService;
            _failedInventoryValidtion = new List<InventoryValidation>();
        }


        public void Visit(ILineItem lineItem)
        {
            // if the line item does not have a product variant reference this vistor cannot check it 
            if (!lineItem.ExtendedData.ContainsProductVariantId()) return;           

            // if the variants inventory is not tracked or if out of stock purchases are allowed check is not necessary
            if (!lineItem.ExtendedData.GetTrackInventoryValue() || lineItem.ExtendedData.GetOutOfStockPurchaseValue()) return;

            var variant = _productVariantService.GetById(lineItem.ExtendedData.GetProductVariantId());

            if (variant == null) return;

            if (variant.TotalInventoryCount < lineItem.Quantity)
            {
                _failedInventoryValidtion.Add(new InventoryValidation(lineItem, variant.TotalInventoryCount));
            }
        }


        public IEnumerable<InventoryValidation> FailedInventoryValidations { get { return _failedInventoryValidtion; } }


        public bool PassesInventoryRequirements {
            get { return !_failedInventoryValidtion.Any(); }
        }


        public class InventoryValidation
        {
            public InventoryValidation(ILineItem requested, int inventoryCount)
            {
                Requested = requested;
                InventoryCount = inventoryCount;
            }

            public ILineItem Requested { get; private set; }
            public int InventoryCount { get; private set; }
        }
    }
}