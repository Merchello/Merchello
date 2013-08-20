using System;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
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
            _residentialMock = TypeFieldMock.AddressTypeResidential;
            _commercialMock = TypeFieldMock.AddressTypeCommercial;
            _customMock = new TypeField("Custom", "Custom", new Guid("A9C5D25C-C825-49F7-B532-14202B8EE61C"));
        }


        /// <summary>
        /// Asserts the AddressType class returns the expected residential configuration
        /// </summary>
        [Test]
        public void AddressType_residential_matches_configuration()
        {
            var type = new AddressTypeField().Residential;

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
            var type = new AddressTypeField().Commercial;

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
            var type = new AddressTypeField().Custom("Custom");

            Assert.AreEqual(_customMock.Alias, type.Alias);
            Assert.AreEqual(_customMock.Name, type.Name);
            Assert.AreEqual(_customMock.TypeKey, type.TypeKey);
        }



    }
}
