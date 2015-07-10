namespace Merchello.Tests.UnitTests.DataModifiers
{
    using System;
    using System.Reflection;

    using Merchello.Core;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Web.Models.ContentEditing;

    using NUnit.Framework;

    [TestFixture]
    public class DataModifierLookupTests
    {
        [Test]
        public void Can_Find_ProductDisplay_Price_Property()
        {
            //// Arrange
            var product = MockProductDataMaker.MockProductDisplayForInserting();

            //// Act
            var prop = product.GetType().GetProperty("Price", BindingFlags.Public | BindingFlags.Instance);

            //// Assert
            Assert.NotNull(prop);
        }

        [Test]
        public void Can_Find_An_Update_ProductDisplay_Price()
        {
            //// Arrange
            var product = MockProductDataMaker.MockProductDisplayForInserting();
            var oldPrice = product.Price;

            //// Act
            var propInfo = product.GetType().GetProperty("Price", BindingFlags.Public | BindingFlags.Instance);
            if (propInfo != null && propInfo.CanRead)
            {
                propInfo.SetValue(product, 20M, null);
            }

            //// Assert
            Console.WriteLine("Old price: {0}", oldPrice);
            Assert.AreNotEqual(oldPrice, product.Price);
            Assert.AreEqual(20M, product.Price);
        }
    }
}