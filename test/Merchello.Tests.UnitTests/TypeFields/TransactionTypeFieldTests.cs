using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{
    [TestFixture]
    [Category("TypeField")]
    public class AppliedPaymentTypeFieldTests
    {
        private ITypeField _mockTransactionCredit;
        private ITypeField _mockTransactionDebit;

        [SetUp]
        public void Setup()
        {
            _mockTransactionCredit = TypeFieldMock.AppliedPaymentCredit;
            _mockTransactionDebit = TypeFieldMock.AppliedPaymentDebit;
        }


        /// <summary>
        /// Asserts the TransactionType Debit returns the expected transaction configuration
        /// </summary>
        [Test]
        public void AppliedPaymentType_debit_matches_configuration()
        {
            var type = EnumTypeFieldConverter.AppliedPayment.Debit;

            Assert.AreEqual(_mockTransactionDebit.Alias, type.Alias);
            Assert.AreEqual(_mockTransactionDebit.Name, type.Name);
            Assert.AreEqual(_mockTransactionDebit.TypeKey, type.TypeKey);

        }


        /// <summary>
        /// Asserts the TransactionType Credit returns the expected wishlist configuration
        /// </summary>
        [Test]
        public void AppliedPayment_Credit_matches_configuration()
        {
            var type = EnumTypeFieldConverter.AppliedPayment.Credit;

            Assert.AreEqual(_mockTransactionCredit.Alias, type.Alias);
            Assert.AreEqual(_mockTransactionCredit.Name, type.Name);
            Assert.AreEqual(_mockTransactionCredit.TypeKey, type.TypeKey);

        }
    }
}
