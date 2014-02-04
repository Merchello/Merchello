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

        private ITypeField _shippingMock;
        private ITypeField _billingMock;
        private ITypeField _customMock;

        [SetUp]
        public void Setup()
        {
            _shippingMock = TypeFieldMock.AddressTypeShipping;
            _billingMock = TypeFieldMock.AddressTypeBilling;
            _customMock = new TypeField("Custom", "Custom", new Guid("A9C5D25C-C825-49F7-B532-14202B8EE61C"));
        }


        /// <summary>
        /// Asserts the AddressType class returns the expected residential configuration
        /// </summary>
        [Test]
        public void AddressType_shipping_matches_configuration()
        {
            var type = EnumTypeFieldConverter.Address.Shipping;

            Assert.AreEqual(_shippingMock.Alias, type.Alias);
            Assert.AreEqual(_shippingMock.Name, type.Name);
            Assert.AreEqual(_shippingMock.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the AddressType class returns the expected commercial configuration
        /// </summary>
        [Test]
        public void AddressType_billing_matches_configuration()
        {
            var type = EnumTypeFieldConverter.Address.Billing;

            Assert.AreEqual(_billingMock.Alias, type.Alias);
            Assert.AreEqual(_billingMock.Name, type.Name);
            Assert.AreEqual(_billingMock.TypeKey, type.TypeKey);
        }

        // TODO Version 1.0 will not expose custom types
        ///// <summary>
        ///// Asserts the AddressType class returns the expected custom configuration
        ///// </summary>
        //[Test]
        //public void AddressType_custom_matches_configuration()
        //{
        //    var type = EnumTypeFieldConverter.Address().Custom("Custom");

        //    Assert.AreEqual(_customMock.Alias, type.Alias);
        //    Assert.AreEqual(_customMock.Name, type.Name);
        //    Assert.AreEqual(_customMock.TypeKey, type.TypeKey);
        //}

    }
}
