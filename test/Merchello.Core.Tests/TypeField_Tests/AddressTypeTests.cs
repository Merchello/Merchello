using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Core.Tests.TypeField_Tests
{
    [TestFixture]
    public class AddressTypeTests
    {

        private ITypeField _residentialMock;
        private ITypeField _commercialMock;

        [SetUp]
        public void Setup()
        {
            _residentialMock = new TypeField("Residential", "Residential", new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46"));
            _commercialMock = new TypeField("Commercial", "Commercial", new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360"));
        }

        /// <summary>
        /// Verifies customer address as two configuration options
        /// </summary>
        [Test]
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
            var type = AddressType.Residential;

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
            var type = AddressType.Commercial;

            Assert.AreEqual(_commercialMock.Alias, type.Alias);
            Assert.AreEqual(_commercialMock.Name, type.Name);
            Assert.AreEqual(_commercialMock.TypeKey, type.TypeKey);
        }

    }
}
