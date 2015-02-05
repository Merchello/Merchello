using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{
    using System;

    using Merchello.Core;

    [TestFixture]
    [Category("TypeField")]
    public class LineItemTypeFieldTests
    {
        private ITypeField _mockProduct;
        private ITypeField _mockShipping;
        private ITypeField _mockTax;
        private ITypeField _mockDiscount;

        [SetUp]
        public void Setup()
        {
            _mockProduct = TypeFieldMock.LineItemProduct;
            _mockShipping = TypeFieldMock.LineItemShipping;
            _mockTax = TypeFieldMock.LineItemTax;
            _mockDiscount = TypeFieldMock.LineItemDiscount;
        }

        /// <summary>
        /// Asserts the LineItemTpe class returns the expected product configuration
        /// </summary>
        [Test]
        public void LineItemType_product_matches_configuration()
        {
            var type = EnumTypeFieldConverter.LineItemType.Product;

            Assert.AreEqual(_mockProduct.Alias, type.Alias);
            Assert.AreEqual(_mockProduct.Name, type.Name);
            Assert.AreEqual(_mockProduct.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the LineItemTpe class returns the expected shipping configuration
        /// </summary>
        [Test]
        public void LineItemType_shipping_matches_configuration()
        {
            var type = EnumTypeFieldConverter.LineItemType.Shipping;

            Assert.AreEqual(_mockShipping.Alias, type.Alias);
            Assert.AreEqual(_mockShipping.Name, type.Name);
            Assert.AreEqual(_mockShipping.TypeKey, type.TypeKey);
        }


        /// <summary>
        /// Asserts the LineItemTpe class returns the expected tax configuration
        /// </summary>
        [Test]
        public void LineItemType_tax_matches_configuration()
        {
            var type = EnumTypeFieldConverter.LineItemType.Tax;

            Assert.AreEqual(_mockTax.Alias, type.Alias);
            Assert.AreEqual(_mockTax.Name, type.Name);
            Assert.AreEqual(_mockTax.TypeKey, type.TypeKey);
        }


        /// <summary>
        /// Asserts the LineItemType class returns the expected discount configuration
        /// </summary>
        [Test]
        public void LineItemType_discount_matches_configuration()
        {
            var type = EnumTypeFieldConverter.LineItemType.Discount;

            Assert.AreEqual(_mockDiscount.Alias, type.Alias);
            Assert.AreEqual(_mockDiscount.Name, type.Name);
            Assert.AreEqual(_mockDiscount.TypeKey, type.TypeKey);
        }


        [Test]
        public void Can_Retrieve_A_CustomLineItemTypeField()
        {
            var type = EnumTypeFieldConverter.LineItemType.Custom("CcFee");

            Assert.AreNotEqual(Guid.Empty, type.TypeKey);
        }

        [Test]
        public void Custom_LineItemTypes_Are_Associated_With_CustomInEnum()
        {
            //// Arrange
            var type = EnumTypeFieldConverter.LineItemType.Custom("CcFee");
            Assert.AreNotEqual(Guid.Empty, type.TypeKey);

            //// Act
            var lineItemType = EnumTypeFieldConverter.LineItemType.GetTypeField(type.TypeKey);

            //// Assert
            Assert.AreEqual(LineItemType.Custom, lineItemType);
        }
    }
}
