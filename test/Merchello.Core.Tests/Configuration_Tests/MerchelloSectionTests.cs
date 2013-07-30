using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using NUnit.Framework;

namespace Merchello.Core.Tests.Configuration_Tests
{
    [TestFixture]
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
        /// Verifies that the Guid associated with the Residential CustomerAddress is correctly retrieved from the configuration
        /// </summary>
        [Test]
        public void Customer_Address_Residential_Guid_Matches()
        {
            var guid = new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46");

            var residentialType = _config.TypeFields.CustomerAddress[CustomerAddress.Residential.ToString()];

            Assert.AreEqual(guid, residentialType.TypeKey);            
        }

        /// <summary>
        /// Verifies that the InvoiceItem collection has 5 elements
        /// </summary>
        [Test]
        public void Invoice_Item_Collection_Contains_5_Elements()
        {
            Assert.AreEqual(5, _invoiceItems.Count);
        }

        /// <summary>
        /// Verifies that descriptive name field is read correctly
        /// </summary>
        [Test]
        public void Invoice_Item_Charge_Name_Is_Charge()
        {
            const string validText = "Charge or Fee";

            var chargeType = _invoiceItems["Charge"];

            Assert.AreEqual(validText, chargeType.Name);
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

    }


}