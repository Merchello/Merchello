namespace Merchello.Core.Chains.InvoiceCreation.CheckoutManager
{
    using System;
    using System.Linq;

    using Merchello.Core.Checkout;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Responsible for apply taxes to invoice tax.
    /// </summary>
    internal class ApplyTaxesToInvoiceTask : CheckoutManagerInvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTaxesToInvoiceTask"/> class.
        /// </summary>
        /// <param name="checkoutManager">
        /// The checkout manager.
        /// </param>
        public ApplyTaxesToInvoiceTask(ICheckoutManagerBase checkoutManager)
            : base(checkoutManager)
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
            if (this.CheckoutManager.Context.ApplyTaxesToInvoice)
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
                    var taxes = value.CalculateTaxes(CheckoutManager.Context.MerchelloContext, taxAddress);
                    this.SetTaxableSetting(value, true);

                    var taxLineItem = taxes.AsLineItemOf<InvoiceLineItem>();

                    var currencyCode =
                        this.CheckoutManager.Context.Services.StoreSettingService.GetByKey(
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
            if (!this.CheckoutManager.Context.Gateways.Taxation.ProductPricingEnabled) return;

            foreach (var item in invoice.Items.Where(x => x.ExtendedData.TaxIncludedInProductPrice()))
            {
                item.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.Taxable, taxable.ToString());
            }
        }
    }
}