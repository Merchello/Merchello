namespace Merchello.Core.Gateways.Notification.Triggering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.MonitorModels;
    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// The order shipped trigger helper.
    /// </summary>
    /// <remarks>
    /// http://issues.merchello.com/youtrack/issue/M-532
    /// </remarks>
    internal class ShipmentResultNotifyModelFactory
    {
        /// <summary>
        /// The <see cref="IOrderService"/>.
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        /// The <see cref="IInvoiceService"/>.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The <see cref="IStoreSettingService"/>.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentResultNotifyModelFactory"/> class.
        /// </summary>
        public ShipmentResultNotifyModelFactory()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentResultNotifyModelFactory"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public ShipmentResultNotifyModelFactory(IMerchelloContext merchelloContext)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");

            _invoiceService = merchelloContext.Services.InvoiceService;
            _orderService = merchelloContext.Services.OrderService;
            _storeSettingService = merchelloContext.Services.StoreSettingService;
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="contacts">
        /// The collection of contact addresses
        /// </param>
        /// <returns>
        /// The <see cref="IShipmentResultNotifyModel"/>.
        /// </returns>
        public IShipmentResultNotifyModel Build(IShipment shipment, IEnumerable<string> contacts)
        {
            var notifyModel = new ShipmentResultNotifyModel()
                                  {
                                      Shipment = shipment,
                                      Contacts = contacts == null ? new string[] { } : contacts.ToArray()
                                    };

            var item = shipment.Items.FirstOrDefault(x => !x.ContainerKey.Equals(Guid.Empty));
            if (item != null)
            {
                var orderKey = item.ContainerKey;

                var order = _orderService.GetByKey(orderKey);

                if (order != null)
                {
                    var invoice = _invoiceService.GetByKey(order.InvoiceKey);
                    if (invoice != null)
                    {
                        notifyModel.Invoice = invoice;
                        if (invoice.Items.Any())
                        {
                            var currencyCode =
                                invoice.Items.First()
                                    .ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.CurrencyCode);
                            var currency = _storeSettingService.GetCurrencyByCode(currencyCode);
                            notifyModel.CurrencySymbol = currency != null ? currency.Symbol : "£";
                        }
                    }
                }
            }

            return notifyModel;
        }
    }
}