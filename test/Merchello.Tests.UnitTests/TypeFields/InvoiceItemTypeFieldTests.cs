using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{

    [TestFixture]
    [Category("TypeField")]
    public class InvoiceItemTypeFieldTests
    {
        private ITypeField _mockProduct;
        private ITypeField _mockCharge;
        private ITypeField _mockShipping;
        private ITypeField _mockTax;
        private ITypeField _mockCredit;

        [SetUp]
        public void Setup()
        {
            _mockProduct = TypeFieldMock.InvoiceItemProduct;
            _mockCharge = TypeFieldMock.InvoiceItemCharge;
            _mockShipping = TypeFieldMock.InvoiceItemShipping;
            _mockTax = TypeFieldMock.InvoiceItemTax;
            _mockCredit = TypeFieldMock.InvoiceItemCredit;
        }

        /// <summary>
        /// Verifies invoice item types should have 5 configuration options
        /// </summary>
        [Test]
        public void InvoiceItemType_should_have_5_options()
        {
            var fields =
                ((MerchelloSection)ConfigurationManager.GetSection("merchello")).TypeFields.InvoiceItem;

            Assert.AreEqual(5, fields.Count);
        }

        /// <summary>
        /// Asserts the InvoiceItemTpe class returns the expected product configuration
        /// </summary>
        [Test]
        public void InvoiceItemType_product_matches_configuration()
        {
            var type = InvoiceItemTypeField.Product;

            Assert.AreEqual(_mockProduct.Alias, type.Alias);
            Assert.AreEqual(_mockProduct.Name, type.Name);
            Assert.AreEqual(_mockProduct.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the InvoiceItemTpe class returns the expected charge configuration
        /// </summary>
        [Test]
        public void InvoiceItemType_charge_matches_configuration()
        {
            var type = InvoiceItemTypeField.Charge;

            Assert.AreEqual(_mockCharge.Alias, type.Alias);
            Assert.AreEqual(_mockCharge.Name, type.Name);
            Assert.AreEqual(_mockCharge.TypeKey, type.TypeKey);
        }

        /// <summary>
        /// Asserts the InvoiceItemTpe class returns the expected charge configuration
        /// </summary>
        [Test]
        public void InvoiceItemType_shipping_matches_configuration()
        {
            var type = InvoiceItemTypeField.Shipping;

            Assert.AreEqual(_mockShipping.Alias, type.Alias);
            Assert.AreEqual(_mockShipping.Name, type.Name);
            Assert.AreEqual(_mockShipping.TypeKey, type.TypeKey);
        }


        /// <summary>
        /// Asserts the InvoiceItemTpe class returns the expected charge configuration
        /// </summary>
        [Test]
        public void InvoiceItemType_tax_matches_configuration()
        {
            var type = InvoiceItemTypeField.Tax;

            Assert.AreEqual(_mockTax.Alias, type.Alias);
            Assert.AreEqual(_mockTax.Name, type.Name);
            Assert.AreEqual(_mockTax.TypeKey, type.TypeKey);
        }


        /// <summary>
        /// Asserts the InvoiceItemTpe class returns the expected charge configuration
        /// </summary>
        [Test]
        public void InvoiceItemType_credit_matches_configuration()
        {
            var type = InvoiceItemTypeField.Credit;

            Assert.AreEqual(_mockCredit.Alias, type.Alias);
            Assert.AreEqual(_mockCredit.Name, type.Name);
            Assert.AreEqual(_mockCredit.TypeKey, type.TypeKey);
        }

    }
}
