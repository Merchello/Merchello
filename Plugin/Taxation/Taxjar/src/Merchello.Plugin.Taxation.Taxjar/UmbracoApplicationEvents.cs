namespace Merchello.Plugin.Taxation.Taxjar
{
    using System;
    using System.Linq;
    using Core;
    using Core.Events;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Sales;
    using Core.Services;
    using Models;
    //using Models.Address;
    using Provider;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Handles Umbraco application events.
    /// </summary>
    public class UmbracoApplicationEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Handles the Umbraco application started event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<UmbracoApplicationEvents>("Initializing TaxJar provider registration binding events");

            GatewayProviderService.Saving += GatewayProviderServiceOnSaving;

            //AutoMapper.Mapper.CreateMap<IValidatableAddress, TaxAddress>();

            //SalePreparationBase.Finalizing += SalePreparationBaseOnFinalizing;
        }

        /// <summary>
        /// The gateway provider service on saving.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> saveEventArgs)
        {
            var key = new Guid("ddb4b1e5-75ec-4c3f-9a05-a51dbad34b4a");
            var provider = saveEventArgs.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);

            if (provider == null) return;

            provider.ExtendedData.SaveProviderSettings(new TaxJarProviderSettings());
        }

        ///// <summary>
        ///// Handles the <see cref="SalePreparationBase"/> finalizing event.  TaxJar require the "quote" be finalized so that 
        ///// it can actually be recorded for reporting purposes.
        ///// </summary>
        ///// <param name="sender">
        ///// The sender.
        ///// </param>
        ///// <param name="args">
        ///// The args.
        ///// </param>
        //private void SalePreparationBaseOnFinalizing(SalePreparationBase sender, SalesPreparationEventArgs<IPaymentResult> args)
        //{
        //    var result = args.Entity;
        //    var invoice = result.Invoice;

        //    if (!result.Payment.Success) return;

        //    var taxation = MerchelloContext.Current.Gateways.Taxation;

        //    var providerKey = new Guid("ddb4b1e5-75ec-4c3f-9a05-a51dbad34b4a");

        //    IAddress taxAddress = null;
        //    var shippingItems = invoice.ShippingLineItems().ToArray();
        //    if (shippingItems.Any())
        //    {
        //        var shipment = shippingItems.First().ExtendedData.GetShipment<OrderLineItem>();
        //        taxAddress = shipment.GetDestinationAddress();
        //    }

        //    taxAddress = taxAddress ?? invoice.GetBillingAddress();

        //    var taxMethod = taxation.GetTaxMethodForTaxAddress(taxAddress);

        //    // If the taxMethod is not found or 
        //    if (taxMethod == null || !providerKey.Equals(taxMethod.ProviderKey)) return;

        //    var provider = taxation.GetProviderByKey(taxMethod.ProviderKey) as TaxJarTaxationGatewayProvider;

        //    if (provider == null) return;

        //    var taxjarTaxMethod = provider.GetTaxJarTaxationGatewayMethod(taxMethod);

        //    taxjarTaxMethod.CalculateTaxForInvoice(invoice, taxAddress, false);
        //}
    }
}