using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceCreation
{
    internal class ApplyTaxesToInvoiceTax : InvoiceCreationAttemptChainTaskBase
    {
        public ApplyTaxesToInvoiceTax(SalePreparationBase salePreparation) 
            : base(salePreparation)
        { }

        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            // if taxes are not to be applied, skip this step
            if (SalePreparation.ApplyTaxesToInvoice)
            {
                try
                {
                    // clear any current tax lines
                    var removers = value.Items.Where(x => x.LineItemType == LineItemType.Tax);
                    foreach (var remove in removers)
                    {
                        value.Items.Remove(remove);
                    }

                    IAddress taxAddress = null;
                    var shippingItems = value.ShippableItems();
                    if (shippingItems.Any())
                    {
                        var shipment = shippingItems.First().ExtendedData.GetShipment<OrderLineItem>();
                        taxAddress = shipment.GetDestinationAddress();
                    }

                    taxAddress = taxAddress ?? value.GetBillingAddress();

                    var taxes = value.CalculateTaxes(SalePreparation.MerchelloContext, taxAddress);

                    var taxLineItem = taxes.AsLineItemOf<InvoiceLineItem>();

                    var currencyCode =
                        SalePreparation.MerchelloContext.Services.StoreSettingService.GetByKey(
                            Constants.StoreSettingKeys.CurrencyCodeKey).Value;

                    taxLineItem.ExtendedData.SetValue(Constants.ExtendedDataKeys.CurrencyCode, currencyCode);

                    value.Items.Add(taxLineItem);

                    return Attempt<IInvoice>.Succeed(value);
                }
                catch (Exception ex)
                {
                    return Attempt<IInvoice>.Fail(ex);
                }
            }

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}