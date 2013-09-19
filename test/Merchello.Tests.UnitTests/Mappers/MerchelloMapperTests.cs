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
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps ICustomer to CustomerMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_ICustomer_To_CustomerMapper()
        {
            //// Arrange
            var expected = typeof (CustomerMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof (ICustomer));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IAddress to AddressMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IAddress_To_AddressMapper()
        {
            //// Arrange
            var expected = typeof (AddressMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof (IAddress));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IAnonymousCustomer to AnonymousCustomerMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IAnonymousCustomer_To_AnonymousCustomerMapper()
        {
            //// Arrange
            var expected = typeof (AnonymousCustomerMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof (IAnonymousCustomer));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IInvoiceStatus to InvoiceStatusMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IInvoiceStatus_To_InvoiceStatusMapper()
        {
            //// Arrange
            var expected = typeof (InvoiceStatusMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof (IInvoiceStatus));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IInvoice to InvoiceMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IInvoice_To_InvoiceMapper()
        {
            //// Arrage
            var expected = typeof(InvoiceMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IInvoice));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IBasket to BasketMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IBasket_To_BasketMapper()
        {

            //// Arrage
            var expected = typeof(BasketMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IBasket));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IBasketItem to BasketItemMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IBasketItem_To_BasketItemMapper()
        {

            //// Arrage
            var expected = typeof(BasketItemMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IBasketItem));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IInvoiceItem to InvoiceItemMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IInvoiceItem_To_InvoiceItemMapper()
        {

            //// Arrage
            var expected = typeof(InvoiceItemMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IInvoiceItem));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IPayment to PaymentMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IPayment_To_PaymentMapper()
        {

            //// Arrage
            var expected = typeof(PaymentMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IPayment));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IProduct to ProductMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IProduct_To_ProductMapper()
        {

            //// Arrage
            var expected = typeof(ProductMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IProduct));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IShipment to ShipmentMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IShipment_To_ShipmentMapper()
        {

            //// Arrage
            var expected = typeof(ShipmentMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IShipment));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps IShipMethod to ShipMethodMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_IShipMethod_To_ShipMethodMapper()
        {

            //// Arrage
            var expected = typeof(ShipMethodMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(IShipMethod));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }

        /// <summary>
        /// Test to verify <see cref="MerchelloMappers"/> correctly maps ITransaction to TransactionMapper
        /// </summary>
        [Test]
        public void Mapper_Resolves_ITransaction_To_TransactionMapper()
        {

            //// Arrage
            var expected = typeof(TransactionMapper);

            //// Act
            var resolved = MerchelloMappers.ResolveByType(typeof(ITransaction));

            //// Assert
            Assert.IsTrue(resolved.Success);
            Assert.AreSame(expected, resolved.Result.GetType());
        }



    }
}
