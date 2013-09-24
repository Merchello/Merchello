using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Configuration
{
    [TestFixture]
    [Category("Configuration")]
    public class MerchelloSectionTests
    {
        private MerchelloSection _config;
        private TypeFieldCollection _invoiceItems;

        /// <summary>
        /// Setup values for each test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _config = ConfigurationManager.GetSection("merchello") as MerchelloSection;
            _invoiceItems = _config.TypeFields.InvoiceItem;
        }


        /// <summary>
        /// Verifies the enableLogging attribute is accessible
        /// </summary>
        [Test]
        public void EnableLogging_Is_False()
        {
            Assert.IsFalse(_config.EnableLogging);
        }

        /// <summary>
        /// Verifies the defaultCountryCode attribute is accessible
        /// </summary>
        [Test]
        public void DefaultCountryCode_Is_US()
        {
            Assert.AreEqual("US", _config.DefaultCountryCode);
        }

        /// <summary>
        /// Verifies the defaultConnectionString matches Umbraco's connection string name
        /// </summary>
        [Test]
        public void ConnectionString_Is_umbracoDbDSN()
        {
            Assert.AreEqual("umbracoDbDSN", _config.DefaultConnectionStringName);
        }

        private enum CustomerAddress
        {
            Residential,
            Commercial
        }

        /// <summary>
        /// Verifies an empty collection 
        /// </summary>
        [Test]
        public void Product_Item_Collection_Is_Empty()
        {
            var productTypeCollection = _config.TypeFields.Product;

            Assert.IsEmpty(productTypeCollection);
        }

        /// <summary>
        /// Test to verify that the DefaultApplyPaymentStrategy can be retrieved
        /// </summary>
        [Test]
        public void Can_Retrieve_DefaultApplyPaymentStrategy_Setting()
        {
            //// Arrage
            const string expected = "Merchello.Core.OrderFulfillment.Strategies.Payment.SaveAndApplyStrategy, Merchello.Core";

            //// Act
            var actual = _config.Settings["DefaultApplyPaymentStrategy"].Value;

            //// Assert
            Assert.AreEqual(expected, actual);
        }
    }


}