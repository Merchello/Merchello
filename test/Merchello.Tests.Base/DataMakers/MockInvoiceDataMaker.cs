using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockInvoiceDataMaker : MockDataMakerBase
    {
        public static IInvoice InvoiceForInserting(ICustomer customer, IInvoiceStatus invoiceStatus, IAddress address)
        {
            return new Invoice(customer, address, invoiceStatus, GetAmount())
            {
                InvoiceNumber = InvoiceNumber(),
                InvoiceDate = DateTime.Now,
                Exported = false,
                Paid = false,
                Shipped = false,
                BillToName = customer.FullName,
                BillToAddress1 = address.Address1,
                BillToAddress2 = address.Address2,
                BillToLocality = address.Locality,
                BillToRegion = address.Region,
                BillToPostalCode = address.PostalCode,
                BillToPhone = address.Phone,
                BillToCompany = address.Company,
                                
            };
        }

        public static IEnumerable<IInvoice> InvoiceCollectionForInserting(ICustomer customer, IInvoiceStatus invoiceStatus, IAddress address, int count)
        {
            for(var i = 0; i < count; i++) yield return InvoiceForInserting(customer, invoiceStatus, address);
        }


        private static string InvoiceNumber()
        {
            return NoWhammyStop.Next(500000).ToString(CultureInfo.InvariantCulture);
        }


    }
}
