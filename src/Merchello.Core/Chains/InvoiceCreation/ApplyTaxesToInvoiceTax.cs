namespace Merchello.Core.Chains.InvoiceCreation
{
    using System;
    using System.Linq;
    using Models;
    using Sales;
    using Umbraco.Core;

    /// <summary>
    /// Responsible for apply taxes to invoice tax.
    /// </summary>
    internal class ApplyTaxesToInvoiceTax : InvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTaxesToInvoiceTax"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        public ApplyTaxesToInvoiceTax(SalePreparationBase salePreparation)
            : base(salePreparation)
        {            
        }

        /// <summary>
        /// Performs the task of applying taxes to the invoice.
        /// </summary>
        /// <param name="value">
        /// The <see cref="IInvoice"/>
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
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
                    var shippingItems = value.ShippingLineItems().ToArray();
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
                            Core.Constants.StoreSettingKeys.CurrencyCodeKey).Value;

                    taxLineItem.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, currencyCode);

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