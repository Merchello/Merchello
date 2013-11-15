using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together invoice item data for testing
    /// </summary>
    public class MockInvoiceItemDataMaker : MockDataMakerBase
    {

        //public static IInvoiceLineItem InvoiceLineItemForInserting(IInvoice invoice, InvoiceItemType invoiceItemType)
        //{
        //    var typeKey = EnumTypeFieldConverter.InvoiceItem.GetTypeField(invoiceItemType).TypeKey;
        //    return new InvoiceLineItem(invoice.Id, typeKey, ProductItemName(), MockSku(), Quanity(), PriceCheck(), new ExtendedDataCollection());
        //}

        //public static IEnumerable<IInvoiceLineItem> InvoiceLineItemCollectionForInserting(IInvoice invoice, InvoiceItemType invoiceItemType, int count)
        //{
        //    for (var i = 0; i < count; i++) yield return InvoiceLineItemForInserting(invoice, invoiceItemType);
        //}



        

    }
    
}