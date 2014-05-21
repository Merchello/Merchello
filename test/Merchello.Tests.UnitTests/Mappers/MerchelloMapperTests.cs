using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Persistence.Mappers;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Mappers
{
    [TestFixture]
    [Category("Mappers")]
    public class MerchelloMapperTests
    {
        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps ICatalogInventory to the CatalogInventoryMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_ICatalogInventor_To_CatalogInventoryMapper()
        {
            //// Arrange
            var expected = typeof(CatalogInventoryMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(ICatalogInventory));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps ICountryTaxRate to the CountryTaxRateMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_ICountryTaxRate_To_CountryTaxRateMapper()
        {
            //// Arrange
            var expected = typeof(TaxMethodMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(ITaxMethod));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());   
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps ICustomer to CustomerMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_ICustomer_To_CustomerMapper()
        {
            //// Arrange
            var expected = typeof (CustomerMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof (ICustomer));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IAddress to AddressMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IAddress_To_AddressMapper()
        {
            //// Arrange
            var expected = typeof (CustomerAddressMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(ICustomerAddress));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IAnonymousCustomer to AnonymousCustomerMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IAnonymousCustomer_To_AnonymousCustomerMapper()
        {
            //// Arrange
            var expected = typeof (AnonymousCustomerMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IAnonymousCustomer));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IGatewayProvider to GatewayProviderMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IGatewayProvider_To_GatewayProviderMapper()
        {
            //// Arrange
            var expected = typeof(GatewayProviderMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IGatewayProviderSettings));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IInvoice to InvoiceMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IInvoice_To_InvoiceMapper()
        {
            //// Arrange
            var expected = typeof(InvoiceMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IInvoice));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IInvoiceStatus to InvoiceStatusMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IInvoiceStatus_To_InvoiceStatusMapper()
        {
            //// Arrange
            var expected = typeof (InvoiceStatusMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IInvoiceStatus));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }


        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IItemCache to ItemCacheMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IItemCache_To_ItemCacheMapper()
        {

            //// Arrage
            var expected = typeof(ItemCacheMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IItemCache));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IBasketItem to BasketItemMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IItemCacheLineItem_To_ItemCacheLineItemMapper()
        {

            //// Arrage
            var expected = typeof(ItemCacheLineItemMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IItemCacheLineItem));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IInvoiceItem to InvoiceItemMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IInvoiceLineItem_To_InvoiceItemMapper()
        {

            //// Arrage
            var expected = typeof(InvoiceLineItemMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IInvoiceLineItem));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }


        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IOrder to OrdertMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IOrder_To_OrderMapper()
        {

            //// Arrage
            var expected = typeof(OrderMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IOrder));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }


        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IPayment to PaymentMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IPayment_To_PaymentMapper()
        {

            //// Arrage
            var expected = typeof(PaymentMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IPayment));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IPaymentMethod to PaymentMethodMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IPayment_To_PaymentMethodMapper()
        {

            //// Arrage
            var expected = typeof(PaymentMethodMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IPaymentMethod));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IProduct to ProductMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IProduct_To_ProductMapper()
        {

            //// Arrage
            var expected = typeof(ProductMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IProduct));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IProductVariant to ProductVariantMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IProductVariant_To_ProductVariantMapper()
        {

            //// Arrage
            var expected = typeof(ProductVariantMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IProductVariant));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IShipCountry to ShipCountryMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IShipCountry_To_ShipCountryMapper()
        {

            //// Arrage
            var expected = typeof(ShipCountryMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IShipCountry));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IShipCountry to ShipCountryMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IShipRateTier_To_ShipRateTierMapper()
        {

            //// Arrage
            var expected = typeof(ShipRateTierMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IShipRateTier));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IShipment to ShipmentMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IShipment_To_ShipmentMapper()
        {

            //// Arrage
            var expected = typeof(ShipmentMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IShipment));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IShipMethod to ShipMethodMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IShipMethod_To_ShipMethodMapper()
        {

            //// Arrage
            var expected = typeof(ShipMethodMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IShipMethod));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps ITransaction to TransactionMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_ITransaction_To_TransactionMapper()
        {

            //// Arrage
            var expected = typeof(AppliedPaymentMapper);

            //// Act
            var resolved = MerchelloMapper.Current.ResolveByType(typeof(IAppliedPayment));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        ///// <summary>
        ///// Test to verify <see cref="MerchelloMapper "/> correctly maps IRegisteredGatewayProvider to RegisteredGatewayProviderMapper
        ///// </summary>
        //[Test]
        //public void Mapper_Resolves_IRegisteredGatewayProvider_To_RegisteredGatewayProviderMapper()
        //{
        //    //// Arrage
        //    var expected = typeof(GatewayProviderMapper);

        //    //// Act
        //    var resolved = MerchelloMapper.Current.ResolveByType(typeof(IGatewayProvider));

        //    //// Assert
        //    Assert.IsTrue(resolved.Success);
        //    Assert.AreSame(expected, resolved.Result.GetType());
        //}

    }
}
