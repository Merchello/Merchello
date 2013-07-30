using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using NUnit.Framework;

namespace Merchello.Core.Tests.Configuration_Tests
{
    [TestFixture]
    public class MerchelloSectionConfigurationSection
    {
        private MerchelloSection _config;
        private TypeFieldCollection _invoiceItems;

        [SetUp]
        public void Setup()
        {
            _config = ConfigurationManager.GetSection("merchello") as MerchelloSection;
            _invoiceItems = _config.TypeFields.InvoiceItem;
        }

        [Test]
        public void Customer_Address_Residential_Guid_Matches()
        {
            var guid = new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46");
            Assert.AreEqual(guid, _config.TypeFields.CustomerAddress[CustomerAddress.Residential.ToString()].TypeKey);            
        }

        [Test]
        public void Invoice_Item_Collection_Contains_5_Elements()
        {
            Assert.AreEqual(5, _invoiceItems.Count);
        }


        [Test]
        public void Invoice_Item_Charge_Name_Is_Charge()
        {
            Assert.AreEqual("Charge or Fee", _invoiceItems["Charge"].Name);
        }

        [Test]
        public void Product_Item_Collection_Is_Empty()
        {
            Assert.IsEmpty(_config.TypeFields.Product);
        }

        private enum CustomerAddress
        {
            Residential,
            Commercial
        }

    }


}