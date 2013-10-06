using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{
    [TestFixture]
    [Category("TypeField")]
    public class PaymentMethodTypeFieldTests
    {

        private ITypeField _cashMock;
        private ITypeField _creditCardMock;
        private ITypeField _purchaseOrderMock;

        [SetUp]
        public void Setup()
        {
            _cashMock = TypeFieldMock.PaymentMethodCash;
            _creditCardMock = TypeFieldMock.PaymentMethodCreditCard;
            _purchaseOrderMock = TypeFieldMock.PaymentMethodPurchaseOrder;
        }


        /// <summary>
        /// Asserts the PaymentMethodTypeField class returns the expected cash configuration
        /// </summary>
        [Test]
        public void PaymentMethodType_cash_matches_configuration()
        {
            var type = EnumTypeFieldConverter.PaymentMethod.Cash;

            Assert.AreEqual(_cashMock.Alias, type.Alias);
            Assert.AreEqual(_cashMock.Name, type.Name);
            Assert.AreEqual(_cashMock.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the PaymentMethodTypeField class returns the expected credit card configuration
        /// </summary>
        [Test]
        public void PaymentMethodType_credit_card_matches_configuration()
        {
            var type = EnumTypeFieldConverter.PaymentMethod.CreditCard;

            Assert.AreEqual(_creditCardMock.Alias, type.Alias);
            Assert.AreEqual(_creditCardMock.Name, type.Name);
            Assert.AreEqual(_creditCardMock.TypeKey, type.TypeKey);

        }

        /// <summary>
        /// Asserts the PaymentMethodTypeField class returns the expected purchase order configuration
        /// </summary>
        [Test]
        public void PaymentMethodType_purchase_order_matches_configuration()
        {
            var type = EnumTypeFieldConverter.PaymentMethod.PurchaseOrder;

            Assert.AreEqual(_purchaseOrderMock.Alias, type.Alias);
            Assert.AreEqual(_purchaseOrderMock.Name, type.Name);
            Assert.AreEqual(_purchaseOrderMock.TypeKey, type.TypeKey);

        }

    }


}
