using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Core.Tests.TypeField_Tests
{
    public class InvoiceItemTypeTests
    {
        private ITypeField _mockProduct;
        private ITypeField _mockCharge;
        private ITypeField _mockShipping;
        private ITypeField _mockTax;
        private ITypeField _mockCredit;

        [SetUp]
        public void Setup()
        {
            _mockProduct = new TypeField("Product", "Product", new Guid("576CB1FB-5C0D-45F5-8CCD-94F63D174902"));
            _mockCharge = new TypeField("Charge", "Charge or Fee", new Guid("5574BB84-1C96-4F7E-91FB-CFD7C11162A0"));
            _mockShipping = new TypeField("Shipping", "Shipping", new Guid("7E69FFD2-394C-44BF-9442-B86F67AEC110"));
            _mockTax = new TypeField("Tax", "Tax", new Guid("3F4830C8-FB7C-4393-831D-3953525541B3"));
            _mockCredit = new TypeField("Credit", "Credit", new Guid("18DEF584-E92A-42F5-9F6F-A49034DAB34F"));
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
            var type = InvoiceItemType.Product;

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
            var type = InvoiceItemType.Charge;

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
            var type = InvoiceItemType.Shipping;

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
            var type = InvoiceItemType.Tax;

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
            var type = InvoiceItemType.Credit;

            Assert.AreEqual(_mockCredit.Alias, type.Alias);
            Assert.AreEqual(_mockCredit.Name, type.Name);
            Assert.AreEqual(_mockCredit.TypeKey, type.TypeKey);
        }

    }
}
