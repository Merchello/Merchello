using Merchello.Core.Models;
using Merchello.Core.Persistence.Mappers;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Mappers
{
    [TestFixture]
    [Category("Mappers")]
    public class MerchelloMapperTests
    {
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

        ///// <summary>
        ///// Test to verify <see cref="MerchelloMapper"/> correctly maps IInvoice to InvoiceMapper
        ///// </summary>
        //[Test]
        //public void Mapper_Resolves_IInvoice_To_InvoiceMapper()
        //{
        //    //// Arrage
        //    var expected = typeof(InvoiceMapper);

        //    //// Act
        //    var resolved = MerchelloMapper.Current.ResolveByType(typeof(IInvoice));

        //    //// Assert
        //    Assert.IsTrue(resolved.Success);
        //    Assert.AreSame(expected, resolved.Result.GetType());
        //}

        /// <summary>
        /// Test to verify <see cref="MerchelloMapper"/> correctly maps IBasket to BasketMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IBasket_To_BasketMapper()
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
        public void Mapper_Resolves_ICustomerItemCache_To_ICustomerItemCachetMapper()
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
