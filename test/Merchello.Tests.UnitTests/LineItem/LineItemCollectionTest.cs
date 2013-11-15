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
    [NUnit.Framework.Category("LineItemCollection")]
    public class LineItemCollectionTest
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
        
    }

    
}