using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{
    [TestFixture]
    [Category("TypeField")]
    public class TransactionTypeFieldTests
    {
        private ITypeField _mockTransactionCredit;
        private ITypeField _mockTransactionDebit;

        [SetUp]
        public void Setup()
        {
            _mockTransactionCredit = TypeFieldMock.TransactionCredit;
            _mockTransactionDebit = TypeFieldMock.TransactionDebit;
        }


        /// <summary>
        /// Asserts the TransactionType Debit returns the expected transaction configuration
        /// </summary>
        [Test]
        public void TransactionType_debit_matches_configuration()
        {
            var type = EnumeratedTypeFieldConverter.Transaction().Debit;

            Assert.AreEqual(_mockTransactionDebit.Alias, type.Alias);
            Assert.AreEqual(_mockTransactionDebit.Name, type.Name);
            Assert.AreEqual(_mockTransactionDebit.TypeKey, type.TypeKey);

        }


        /// <summary>
        /// Asserts the TransactionType Credit returns the expected wishlist configuration
        /// </summary>
        [Test]
        public void BasketType_wishlist_matches_configuration()
        {
            var type = EnumeratedTypeFieldConverter.Transaction().Credit;

            Assert.AreEqual(_mockTransactionCredit.Alias, type.Alias);
            Assert.AreEqual(_mockTransactionCredit.Name, type.Name);
            Assert.AreEqual(_mockTransactionCredit.TypeKey, type.TypeKey);

        }
    }
}
