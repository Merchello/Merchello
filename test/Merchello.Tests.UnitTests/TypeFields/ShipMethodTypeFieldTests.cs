using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
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
            _mockFlatRate = TypeFieldMock.ShipMethodFlatRate;
            _mockPercentTotal = TypeFieldMock.ShipMethodPercentTotal;
            _mockCarrier = TypeFieldMock.ShipMethodCarrier;
        }

        /// <summary>
        /// Asserts the ShipMethodType class returns the expected flat rate configuration
        /// </summary>
        [Test]
        public void ShipMethodType_flat_rate_matches_configuration()
        {
            var type = EnumTypeFieldConverter.ShipmentMethod.FlatRate;

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
            var type = EnumTypeFieldConverter.ShipmentMethod.PercentTotal;

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
            var type = EnumTypeFieldConverter.ShipmentMethod.Carrier;

            Assert.AreEqual(_mockCarrier.Alias, type.Alias);
            Assert.AreEqual(_mockCarrier.Name, type.Name);
            Assert.AreEqual(_mockCarrier.TypeKey, type.TypeKey);

        }

    }
}
