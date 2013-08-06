using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Core.Tests.TypeField_Tests
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
            _cashMock = new TypeField("Cash", "Cash", new Guid("9C9A7E61-D79C-4ECC-B0E0-B2A502F252C5"));
            _creditCardMock = new TypeField("CreditCard", "Credit Card", new Guid("CB1354FE-B30C-449E-BD5C-CD50BCBD804A"));
            _purchaseOrderMock = new TypeField("PurchaseOrder", "Purchase Order", new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E"));
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
