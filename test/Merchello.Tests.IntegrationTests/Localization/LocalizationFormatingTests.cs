using System;
using System.Globalization;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Localization
{
    [TestFixture]
    public class LocalizationFormatingTests :  MerchelloAllInTestBase
    {

        [Test]
        public void Can_Format_Currency_BasedOn_US_CountryCode()
        {
            //// Arrange
            var lineItem = new ItemCacheLineItem(LineItemType.Product, "Product", "Sku", 10, 100);

            //// Act
            var price = lineItem.FormatPrice();

            Console.Write(price);

        }
         
    }
}