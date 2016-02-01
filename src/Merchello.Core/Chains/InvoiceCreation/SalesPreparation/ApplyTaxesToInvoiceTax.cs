namespace Merchello.Core.Chains.InvoiceCreation.SalesPreparation
{
    using System;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// Responsible for apply taxes to invoice tax.
    /// </summary>
    [Obsolete("Superseded by CheckoutManger.ApplyTaxesToInvoiceTask")]
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
            if (this.SalePreparation.ApplyTaxesToInvoice)
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

                    this.SetTaxableSetting(value);
                    var taxes = value.CalculateTaxes(this.SalePreparation.MerchelloContext, taxAddress);
                    this.SetTaxableSetting(value, true);                    

                    var taxLineItem = taxes.AsLineItemOf<InvoiceLineItem>();

                    var currencyCode =
                        this.SalePreparation.MerchelloContext.Services.StoreSettingService.GetByKey(
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

        /// <summary>
        /// Sets or resets the tax setting.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxable">
        /// The taxable.
        /// </param>
        /// <remarks>
        /// In cases where a product already includes the tax and we still need to calculate taxes for shipping
        /// and custom line items on the invoice we set the taxable setting on the products to false and then set them back
        /// to true after the tax calculation has been completed.
        /// </remarks>
        private void SetTaxableSetting(IInvoice invoice, bool taxable = false)
        {
            if (!this.SalePreparation.MerchelloContext.Gateways.Taxation.ProductPricingEnabled) return;

            foreach (var item in invoice.Items.Where(x => x.ExtendedData.TaxIncludedInProductPrice()))
            {
                item.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.Taxable, taxable.ToString());  
            }
        }
    }
}