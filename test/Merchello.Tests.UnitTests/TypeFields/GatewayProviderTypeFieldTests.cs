using Merchello.Core;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{
    [TestFixture]
    [Category("TypeField")]
    public class GatewayProviderTypeFieldTests
    {
        /// <summary>
        /// Test to verify that the GatewayProviderType.Shipping can be converted into a TypeField
        /// </summary>
        [Test]
        public void Can_Convert_GatewayProviderTypeShipping_ToTypeField()
        {
            //// Arrange
            var expected = TypeFieldMock.GatewayProviderShipping;

            //// Act
            var actual = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Shipping);

            //// Assert
            Assert.AreEqual(expected.TypeKey, actual.TypeKey);
        }

        /// <summary>
        /// Test to verify that the GatewayProviderType.Payment can be converted into a TypeField
        /// </summary>
        [Test]
        public void Can_Convert_GatewayProviderTypePayment_ToTypeField()
        {
            //// Arrange
            var expected = TypeFieldMock.GatewayProviderPayment;

            //// Act
            var actual = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Payment);

            //// Assert
            Assert.AreEqual(expected.TypeKey, actual.TypeKey);
            
        }


        /// <summary>
        /// Test to verify that the GatewayProviderType.Taxation can be converted into a TypeField
        /// </summary>
        [Test]
        public void Can_Convert_GatewayProviderTypeTaxation_ToTypeField()
        {
            //// Arrange
            var expected = TypeFieldMock.GatewayProviderTaxation;

            //// Act
            var actual = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Taxation);

            //// Assert
            Assert.AreEqual(expected.TypeKey, actual.TypeKey);

        }

    }
}