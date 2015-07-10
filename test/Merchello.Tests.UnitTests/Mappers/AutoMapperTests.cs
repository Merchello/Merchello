using Merchello.Core.Models;
using Merchello.Web;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Mappers
{
    using System;

    using Merchello.Core.Gateways.Payment.Cash;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.DataMakers;

    using Moq;

    [TestFixture]
    public class AutoMapperTests
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            AutoMapperMappings.CreateMappings();  
        }

        /// <summary>
        /// Test verifies the a ProductOption can be mapped to a ProductOptionDisplay using the AutoMapper
        /// </summary>
        [Test]
        public void Can_Map_ProductOption_To_ProductOptionDisplay()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
                {
                    new ProductAttribute("One", "One"),
                    new ProductAttribute("Two", "Two"),
                    new ProductAttribute("Three", "Three"),
                    new ProductAttribute("Four", "Four")
                };

            var productOption = new ProductOption("Numbers", true, attributes);

            //// Act
            var productOptionDisplay = AutoMapper.Mapper.Map<ProductOptionDisplay>(productOption);

            //// Assert
            Assert.NotNull(productOptionDisplay);
        }

        [Test]
        public void Can_Map_CashPaymentMethod_To_PaymentMethodDisplay()
        {
            //// Arrange
            var mockGatewayService = new Mock<IGatewayProviderService>();
            var paymentMethod = new Mock<IPaymentMethod>();

            var cash = new CashPaymentGatewayMethod(mockGatewayService.Object, paymentMethod.Object);

            var display = cash.ToPaymentMethodDisplay();

            Assert.NotNull(display);
            Assert.IsNotNullOrEmpty(display.AuthorizePaymentEditorView.EditorView);
            Assert.IsNotNullOrEmpty(display.AuthorizeCapturePaymentEditorView.EditorView);
            Assert.IsNotNullOrEmpty(display.VoidPaymentEditorView.EditorView);
            Assert.IsNotNullOrEmpty(display.RefundPaymentEditorView.EditorView);
        }

        [Test]
        public void Can_Map_ProductDisplay_To_ProductVariantDisplay()
        {
            //// Arrange
            var product = MockProductDataMaker.MockProductDisplayForInserting();
            var key = Guid.NewGuid();
            var variantKey = Guid.NewGuid();
            product.Key = key;
            product.ProductVariantKey = variantKey;

            //// Act
            var variant = AutoMapper.Mapper.Map<ProductVariantDisplay>(product);

            //// Assert
            Assert.NotNull(variant);
            Assert.AreEqual(variantKey, variant.Key, "Variant key did not match");
            Assert.AreEqual(key, variant.ProductKey, "Product key did not match");
        }
    }
}