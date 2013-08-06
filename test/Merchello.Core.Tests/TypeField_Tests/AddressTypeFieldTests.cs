using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Core.Tests.TypeField_Tests
{
    [TestFixture]
    [Category("TypeField")]
    public class AddressTypeFieldTests
    {

        private ITypeField _residentialMock;
        private ITypeField _commercialMock;
        private ITypeField _customMock;

        [SetUp]
        public void Setup()
        {
            _residentialMock = new TypeField("Residential", "Residential", new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46"));
            _commercialMock = new TypeField("Commercial", "Commercial", new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360"));
            _customMock = new TypeField("Custom", "Custom", new Guid("A9C5D25C-C825-49F7-B532-14202B8EE61C"));
        }

        /// <summary>
        /// Verifies customer address as two configuration options
        /// </summary>
        //[Test]
        public void AddressType_should_have_2_options()
        {
            var fields =
                ((MerchelloSection) ConfigurationManager.GetSection("merchello")).TypeFields.CustomerAddress;

            Assert.AreEqual(2, fields.Count);
        }


        /// <summary>
        /// Asserts the AddressType class returns the expected residential configuration
        /// </summary>
        [Test]
        public void AddressType_residential_matches_configuration()
        {
            var type = AddressTypeField.Residential;

            Assert.AreEqual(_residentialMock.Alias, type.Alias);
            Assert.AreEqual(_residentialMock.Name, type.Name);
            Assert.AreEqual(_residentialMock.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the AddressType class returns the expected commercial configuration
        /// </summary>
        [Test]
        public void AddressType_commercial_matches_configuration()
        {
            var type = AddressTypeField.Commercial;

            Assert.AreEqual(_commercialMock.Alias, type.Alias);
            Assert.AreEqual(_commercialMock.Name, type.Name);
            Assert.AreEqual(_commercialMock.TypeKey, type.TypeKey);
        }

        /// <summary>
        /// Asserts the AddressType class returns the expected custom configuration
        /// </summary>
        [Test]
        public void AddressType_custom_matches_configuration()
        {
            var type = AddressTypeField.Custom("Custom");

            Assert.AreEqual(_customMock.Alias, type.Alias);
            Assert.AreEqual(_customMock.Name, type.Name);
            Assert.AreEqual(_customMock.TypeKey, type.TypeKey);
        }



    }
}
