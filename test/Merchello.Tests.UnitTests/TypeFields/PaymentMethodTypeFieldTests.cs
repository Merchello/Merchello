using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
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
        /// Verifies payment method type as 3 configuration options
        /// </summary>
        [Test]
        public void PaymentMethodType_should_have_3_options()
        {
            var fields =
                ((MerchelloSection)ConfigurationManager.GetSection("merchello")).TypeFields.PaymentMethod;

            Assert.AreEqual(3, fields.Count);
        }



        /// <summary>
        /// Asserts the PaymentMethodTypeField class returns the expected cash configuration
        /// </summary>
        [Test]
        public void PaymentMethodType_cash_matches_configuration()
        {
            var type = PaymentMethodTypeField.Cash;

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
            var type = PaymentMethodTypeField.CreditCard;

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
            var type = PaymentMethodTypeField.PurchaseOrder;

            Assert.AreEqual(_purchaseOrderMock.Alias, type.Alias);
            Assert.AreEqual(_purchaseOrderMock.Name, type.Name);
            Assert.AreEqual(_purchaseOrderMock.TypeKey, type.TypeKey);

        }

    }


}
