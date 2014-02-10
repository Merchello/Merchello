using System;
using System.ComponentModel;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Visitors;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.LineItem
{
    [TestFixture]
    [NUnit.Framework.Category("LineItem")]
    public class LineItemTests
    {

        [Test]
        public void Can_Verify_A_Vistor_Visits_Every_Line_item()
        {
            //// Arrange
            const int itemCount = 10;
            var itemCache = new ItemCache(Guid.NewGuid(), ItemCacheType.Basket) { Key = Guid.NewGuid() };
            for (var i = 0; i < itemCount; i++)
            { itemCache.AddItem(MockLineItemDataMaker.MockItemCacheLineItemComplete(itemCache.Key)); }

            //// Act
            var vistor = new MockLineItemVistor();
            itemCache.Items.Accept(vistor);

            //// Assert
            Assert.AreEqual(itemCount, vistor.Visited.Count());
        }
        
        /// <summary>
        /// Test confirms that a line item of type <see cref="ItemCacheLineItem"/> can be converted to a
        /// line item of type <see cref="InvoiceLineItem"/>
        /// </summary>
        [Test]
        public void Can_Convert_A_LineItem_Of_Type_ItemCacheLineItem_To_A_InvoiceLineItem()
        {
            //// Arrange
            var product = MockProductDataMaker.MockProductForInserting();
            var extendedData = new ExtendedDataCollection();
            extendedData.AddProductVariantValues(((Product)product).MasterVariant);
            var itemCacheLineItem = new ItemCacheLineItem(LineItemType.Product, product.Name,
                                                          product.Sku, 2, 2*product.Price, extendedData);

            //// Act
            var invoiceLineItem = itemCacheLineItem.AsLineItemOf<InvoiceLineItem>();

            //// Assert
            Assert.NotNull(invoiceLineItem);
            Assert.AreEqual(Guid.Empty, invoiceLineItem.ContainerKey);
            Assert.AreEqual(typeof(InvoiceLineItem), invoiceLineItem.GetType());

        }
    }

    
}