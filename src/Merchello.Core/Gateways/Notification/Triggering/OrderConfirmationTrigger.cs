namespace Merchello.Core.Gateways.Notification.Triggering
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Models.MonitorModels;
    using Observation;
    using Payment;

    using Umbraco.Core;

    /// <summary>
    /// Represents and OrderConfirmationTrigger
    /// </summary>
    [TriggerFor("OrderConfirmation", Topic.Notifications)]
    public sealed class OrderConfirmationTrigger : NotificationTriggerBase<IPaymentResult, IPaymentResultMonitorModel>
    {
        /// <summary>
        /// The <see cref="IStoreSettingService"/>.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="IShipMethodService"/>.
        /// </summary>
        private readonly IShipMethodService _shipMethodService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderConfirmationTrigger"/> class.
        /// </summary>
        public OrderConfirmationTrigger()
            : this(MerchelloContext.Current)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderConfirmationTrigger"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public OrderConfirmationTrigger(IMerchelloContext merchelloContext)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            _storeSettingService = merchelloContext.Services.StoreSettingService;
            _shipMethodService = ((ServiceContext)merchelloContext.Services).ShipMethodService;
        }

        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="contacts">
        /// An additional list of contacts
        /// </param>
        protected override void Notify(IPaymentResult model, IEnumerable<string> contacts)
        {
            var symbol = string.Empty;
            IShipment shipment = null;
            IShipMethod shipMethod = null;

            if (model.Invoice != null)
            {
                if (model.Invoice.Items.Any())
                {
                    var currencyCode =
                        model.Invoice.Items.First().ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.CurrencyCode);
                    var currency = _storeSettingService.GetCurrencyByCode(currencyCode);
                    symbol = currency.Symbol;
                }


                // get shipping information if any
                
                var shippingLineItems = model.Invoice.ShippingLineItems().ToArray();
                if (shippingLineItems.Any())
                {
                    // just use the first one
                    shipment = shippingLineItems.First().ExtendedData.GetShipment<InvoiceLineItem>();

                    // get the shipmethod information
                    if (shipment != null && shipment.ShipMethodKey.HasValue)
                    {
                        shipMethod = _shipMethodService.GetByKey(shipment.ShipMethodKey.Value);
                    }
                }
            }

            NotifyMonitors(model.ToOrderConfirmationNotification(contacts, shipment, shipMethod, symbol));           
        }
    }
}