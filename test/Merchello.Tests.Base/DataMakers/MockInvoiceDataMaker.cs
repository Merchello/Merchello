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
        public static IInvoice InvoiceForInserting(ICustomer customer, IInvoiceStatus invoiceStatus, ICustomerAddress customerAddress)
        {
            return new Invoice(customer, customerAddress, invoiceStatus, GetAmount())
            {
                InvoiceNumber = InvoiceNumber(),
                InvoiceDate = DateTime.Now,
                Exported = false,
                Paid = false,
                BillToName = customer.FullName,
                BillToAddress1 = customerAddress.Address1,
                BillToAddress2 = customerAddress.Address2,
                BillToLocality = customerAddress.Locality,
                BillToRegion = customerAddress.Region,
                BillToPostalCode = customerAddress.PostalCode,
                BillToPhone = customerAddress.Phone,
                BillToCompany = customerAddress.Company,
                                
            };
        }
        

        public static IEnumerable<IInvoice> InvoiceCollectionForInserting(ICustomer customer, IInvoiceStatus invoiceStatus, ICustomerAddress customerAddress, int count)
        {
            for(var i = 0; i < count; i++) yield return InvoiceForInserting(customer, invoiceStatus, customerAddress);
        }


        private static string InvoiceNumber()
        {
            return NoWhammyStop.Next(500000).ToString(CultureInfo.InvariantCulture);
        }


    }
}
