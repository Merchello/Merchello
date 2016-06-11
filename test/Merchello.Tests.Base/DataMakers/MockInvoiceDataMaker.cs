﻿using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Persistence;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.Mocks;
    using Merchello.Tests.Base.SqlSyntax;

    public class MockInvoiceDataMaker : MockDataMakerBase
    {
        public static IInvoice InvoiceForInserting(IAddress billTo, decimal total)
        {
            var status = new InvoiceStatus()
                {
                    Key = Constants.DefaultKeys.InvoiceStatus.Unpaid,
                    Active = true,
                    Alias = "unpaid",
                    Name = "Unpaid",
                    SortOrder = 0
                };
            var invoice = new Invoice(status) { CurrencyCode = "USD" };
            invoice.SetBillingAddress(billTo);
            invoice.Total = total;

            return invoice;
        }

        public static IInvoice GetMockInvoiceForTaxation()
        {
            var origin = new Address()
            {
                Organization = "Mindfly Web Design Studios",
                Address1 = "114 W. Magnolia St. Suite 300",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US",
                Email = "someone@mindfly.com",
                Phone = "555-555-5555"
            };

            var billToShipTo = new Address()
            {
                Name = "Space Needle",
                Address1 = "400 Broad St",
                Locality = "Seattle",
                Region = "WA",
                PostalCode = "98109",
                CountryCode = "US",
            };

            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);

            // sets up the Umbraco SqlSyntaxProvider Singleton OBSOLETE
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax(syntax);

            var sqlSyntaxProvider = SqlSyntaxProviderTestHelper.SqlSyntaxProvider(syntax);

            var invoiceService = new InvoiceService(TestLogger, sqlSyntaxProvider);

            var invoice = invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            invoice.SetBillingAddress(billToShipTo);

            
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.Taxable, true.ToString());

            // make up some line items
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 10, 1, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Product, "Item 2", "I2", 2, 40, extendedData);

            invoice.Items.Add(l1);
            invoice.Items.Add(l2);

            var shipment = new ShipmentMock(origin, billToShipTo, invoice.Items);

            var shipmethod = new ShipMethodMock();

            var quote = new ShipmentRateQuote(shipment, shipmethod) { Rate = 16.22M };
            invoice.Items.Add(quote.AsLineItemOf<InvoiceLineItem>());

            invoice.Total = invoice.Items.Sum(x => x.TotalPrice);

            return invoice;
        }
    }
}