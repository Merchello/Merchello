using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.OrderCreation
{
    internal class UpdateInventoryAndBackOrderTask : InvoiceAttemptChainTaskBase
    {
        private List<IOrderLineItem> _backOrderLineItems = new List<IOrderLineItem>();

        public UpdateInventoryAndBackOrderTask(IMerchelloContext merchelloContext, IInvoice invoice) 
            : base(merchelloContext, invoice)
        { }

        public override Attempt<IOrder> PerformTask(IOrder value)
        {
            var productLineItems = value.Items.Where(
                x => x.LineItemType == LineItemType.Product && 
                x.ExtendedData.ContainsProductVariantKey() &&
                x.ExtendedData.GetTrackInventoryValue()
                ).ToArray();

            // get the variants to check the inventory
            var variants = MerchelloContext.Services.ProductVariantService.GetByKeys(productLineItems.Select(x => x.ExtendedData.GetProductVariantKey())).ToArray();

            foreach (var lineItem in productLineItems)
            {
                // assert the line item has a WarehouseCatalogKey to know which inventory record to update
                if (!lineItem.ExtendedData.ContainsWarehouseCatalogKey())
                {
                    return
                        Attempt<IOrder>.Fail(
                            new InvalidOperationException(
                                "The order product line item does not contain a WarehouseCatalogKey which is required to update inventory."));
                }

                var catalogKey = lineItem.ExtendedData.GetWarehouseCatalogKey();

                var variant = variants.FirstOrDefault(x => x.Key == lineItem.ExtendedData.GetProductVariantKey());
                
                // assert the variant has not been deleted since it was invoiced.  eg. cash payment finally collected
                if (variant == null)
                {
                    return Attempt<IOrder>.Fail(new NullReferenceException("A product variant does not exist with the key found"));
                }

                var inventory = variant.CatalogInventories.FirstOrDefault(x => x.CatalogKey == catalogKey);

                // assert there is an inventory record
                if (inventory == null) return Attempt<IOrder>.Fail(new NullReferenceException("CatalogInventory not found for product"));

                if (inventory.Count < lineItem.Quantity)
                {
                    var backOrderCount = lineItem.Quantity - inventory.Count;
                    
                    if (backOrderCount == lineItem.Quantity)
                    {
                        ((OrderLineItem) lineItem).BackOrder = true;
                    }
                    else
                    {
                        var backOrder = lineItem.AsLineItemOf<OrderLineItem>();
                        backOrder.Quantity = backOrderCount;
                        backOrder.BackOrder = true;
                        backOrder.Sku += "*";
                        lineItem.Quantity = lineItem.Quantity - backOrderCount;
                        _backOrderLineItems.Add(backOrder);
                    }
                    
                    inventory.Count = 0;
                    
                }
                else
                {
                    inventory.Count = inventory.Count - lineItem.Quantity;
                }
                
            }

            // save the variants with the updated inventories
            MerchelloContext.Services.ProductVariantService.Save(variants);

            // add the backorder lineitems to the order
            foreach (var backOrder in _backOrderLineItems)
            {
                value.Items.Add(backOrder);
            }

            return Attempt<IOrder>.Succeed(value);
        }

        
    }
}