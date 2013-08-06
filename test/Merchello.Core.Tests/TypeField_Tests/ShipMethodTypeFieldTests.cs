using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Core.Tests.TypeField_Tests
{
    [TestFixture]
    [Category("TypeField")]
    public class ShipMethodTypeFieldTests
    {
        private ITypeField _mockFlatRate;
        private ITypeField _mockPercentTotal;
        private ITypeField _mockCarrier;

        [SetUp]
        public void Setup()
        {
            _mockFlatRate = new TypeField("FlatRate", "Flat Rate", new Guid("1D0B73CF-AE9D-4501-83F5-FA0B2FEE1236"));
            _mockPercentTotal = new TypeField("PercentTotal", "Percent of Total", new Guid("B056DA45-3FB0-49AE-8349-6FCEB1465DF6"));
            _mockCarrier = new TypeField("Carrier", "Carrier", new Guid("4311536A-9554-43D4-8422-DEAAD214B469"));
        }

        /// <summary>
        /// Verifies shipment method should have 3 configuration options
        /// </summary>
        [Test]
        public void ShipMethodType_should_have_3_options()
        {
            var fields =
                ((MerchelloSection)ConfigurationManager.GetSection("merchello")).TypeFields.ShipMethod;

            Assert.AreEqual(3, fields.Count);
        }

        /// <summary>
        /// Asserts the ShipMethodType class returns the expected flat rate configuration
        /// </summary>
        [Test]
        public void ShipMethodType_flat_rate_matches_configuration()
        {
            var type = ShipMethodTypeField.FlatRate;

            Assert.AreEqual(_mockFlatRate.Alias, type.Alias);
            Assert.AreEqual(_mockFlatRate.Name, type.Name);
            Assert.AreEqual(_mockFlatRate.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the ShipMethodType class returns the expected percent total configuration
        /// </summary>
        [Test]
        public void ShipMethodType_percent_total_matches_configuration()
        {
            var type = ShipMethodTypeField.PercentTotal;

            Assert.AreEqual(_mockPercentTotal.Alias, type.Alias);
            Assert.AreEqual(_mockPercentTotal.Name, type.Name);
            Assert.AreEqual(_mockPercentTotal.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the ShipMethodType class returns the expected carrier configuration
        /// </summary>
        [Test]
        public void ShipMethodType_carrier_matches_configuration()
        {
            var type = ShipMethodTypeField.Carrier;

            Assert.AreEqual(_mockCarrier.Alias, type.Alias);
            Assert.AreEqual(_mockCarrier.Name, type.Name);
            Assert.AreEqual(_mockCarrier.TypeKey, type.TypeKey);

        }

    }
}
