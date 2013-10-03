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

        public static IInvoiceItem InvoiceItemForInserting(IInvoice invoice, InvoiceItemType invoiceItemType)
        {
            var typeKey = EnumTypeFieldConverter.InvoiceItem.GetTypeField(invoiceItemType).TypeKey;
            return new InvoiceItem(invoice, typeKey, null)
                    {
                        Amount = GetAmount(),
                        Name = ProductItemName(),
                        Quantity = Quanity(),
                        UnitOfMeasureMultiplier = 1,
                        Sku = MockSku(),
                        Exported = false
                    };
        }

        public static IEnumerable<IInvoiceItem> InvoiceItemCollectionForInserting(IInvoice invoice, InvoiceItemType invoiceItemType, int count)
        {
            for (var i = 0; i < count; i++) yield return InvoiceItemForInserting(invoice, invoiceItemType);
        }

        private static int Quanity()
        {
            return NoWhammyStop.Next(10);
        }

        

    }
    
}