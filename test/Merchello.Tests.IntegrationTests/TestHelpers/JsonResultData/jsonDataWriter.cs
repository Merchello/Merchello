namespace Merchello.Tests.IntegrationTests.TestHelpers.JsonResultData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ClientDependency.Core;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.SaleHistory;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class JsonDataWriter : MerchelloAllInTestBase
    {
        //[Test]
        public void OrderJsonResults()
        {
            var merchello = new MerchelloHelper();
            var notFulfilled = Core.Constants.OrderStatus.NotFulfilled;
            Console.WriteLine(notFulfilled);

            var orders = merchello.Query.Order.SearchByOrderStatus(
                Core.Constants.OrderStatus.NotFulfilled,
                1,
                10).Items;

            Console.Write(JsonConvert.SerializeObject(orders));
            
        }

        //[Test]
        public void InvoiceJsonResults()
        {
            var merchello = new MerchelloHelper();

            var invoices = merchello.Query.Invoice.Search(1, 10).Items;

            Console.WriteLine(JsonConvert.SerializeObject(invoices));
        }

        //[Test]
        public void ShipmentJsonResults()
        {
            var shipmentService = MerchelloContext.Current.Services.ShipmentService;

            var shipments = ((ShipmentService)shipmentService).GetAll().Select(x => x.ToShipmentDisplay());

            Console.WriteLine(JsonConvert.SerializeObject(shipments));

        }

        //[Test]
        public void PaymentJsonResults()
        {
            var paymentService = MerchelloContext.Current.Services.PaymentService;

            var payments = ((PaymentService)paymentService).GetAll().Select(x => x.ToPaymentDisplay());

            Console.WriteLine(JsonConvert.SerializeObject(payments));
        }

        //[Test]
        public void DailyAuditLogs()
        {
            var list = new List<AuditLogDisplay>();
            var merchello = new MerchelloHelper();
            var invoice = merchello.Query.Invoice.GetByKey(new Guid("85F6F194-2F74-4CD5-9CE8-98723B50E719"));
            var _auditLogService = MerchelloContext.Current.Services.AuditLogService;

            if (invoice != null)
            {
                Console.WriteLine("Got an invoice");
                var invoiceLogs = _auditLogService.GetAuditLogsByEntityKey(invoice.Key).Select(x => x.ToAuditLogDisplay()).ToArray();

                if (invoiceLogs.Any()) list.AddRange(invoiceLogs);

                foreach (var orderLogs in invoice.Orders.Select(order => _auditLogService.GetAuditLogsByEntityKey(order.Key).Select(x => x.ToAuditLogDisplay()).ToArray()).Where(orderLogs => orderLogs.Any()))
                {
                    list.AddRange(orderLogs);
                }

                var paymentKeys = MerchelloContext.Current.Services.PaymentService.GetPaymentsByInvoiceKey(invoice.Key).Select(x => x.Key);
    
                foreach (var paymentLogs in paymentKeys.Select(x => _auditLogService.GetAuditLogsByEntityKey(x).Select(log => log.ToAuditLogDisplay())).ToArray())
                {
                    list.AddRange(paymentLogs);
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(list.ToSalesHistoryDisplay()));
        }

        //[Test]
        public void AllStoreSettings()
        {
            var settingService = MerchelloContext.Current.Services.StoreSettingService;
            var settings = settingService.GetAll();
            var settingDisplay = new SettingDisplay();
            
            Console.WriteLine(JsonConvert.SerializeObject(settingDisplay.ToStoreSettingDisplay(settings)));
           
        }

        //[Test]
        public void AllCurrencies()
        {
            var settingService = MerchelloContext.Current.Services.StoreSettingService;
            var currencies = settingService.GetAllCurrencies();
            Console.WriteLine(JsonConvert.SerializeObject(currencies));
        }

        //[Test]
        public void AllCountries()
        {
            var settingService = MerchelloContext.Current.Services.StoreSettingService;
            var countries = settingService.GetAllCountries();
            Console.WriteLine(JsonConvert.SerializeObject(countries.Select(x => x.ToCountryDisplay())));
        }

        //[Test]
        public void ShipmentStatuses()
        {
            var shipmentService = MerchelloContext.Current.Services.ShipmentService;
            var statuses = shipmentService.GetAllShipmentStatuses().OrderBy(x => x.SortOrder);
            Console.WriteLine(JsonConvert.SerializeObject(statuses.Select(x => x.ToShipmentStatusDisplay())));
        }
    }
}