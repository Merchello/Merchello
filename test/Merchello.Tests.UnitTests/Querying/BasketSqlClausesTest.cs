using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Querying;
using Merchello.Tests.Base.SqlSyntax;
using Merchello.Tests.Base.TypeFields;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class CustomerItemRegisterSqlClausesTest : BaseUsingSqlServerSyntax<ICustomerItemRegister>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="CustomerItemRegisterDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Base_Basket_Clause()
        {
            //// Arrange
            var id = 12;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerItemRegister]")
                .Where("[merchCustomerItemRegister].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemRegisterDto>()
                .Where<CustomerItemRegisterDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="CustomerItemRegisterItemDto"/> query matches generic "select * ..." query  
        /// </summary>
        [Test]
        public void Can_Verify_Base_BasketItem_Clause()
        {
            //// Arrange
            var id = 125;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerItemRegisterItem]")
                .Where("[merchCustomerItemRegisterItem].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemRegisterItemDto>()
                .Where<CustomerItemRegisterItemDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify the typed <see cref="ICustomerItemRegister"/> sql by consumer key queries
        /// </summary>
        [Test]
        public void Can_Verify_Basket_By_Consumer_Query()
        {
            //// Arrange
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerItemRegister]")
                .Where("[merchCustomerItemRegister].[consumerKey] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemRegisterDto>();

            var query = Query<ICustomerItemRegister>.Builder.Where(x => x.ConsumerKey == key);
            var translated = TranslateQuery(sql, query);

            //// Assert
            Assert.AreEqual(expected.SQL, translated.SQL);
        }

        /// <summary>
        /// Test to verify type <see cref="ICustomerItemRegister"/> sql by consumer and basket type field key
        /// </summary>
        [Test]
        public void Can_Verify_Basket_By_Consumer_And_BasketType_Query()
        {
            //// Arrange
            var basketTypeKey = TypeFieldMock.BasketBasket.TypeKey;
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
               .From("[merchCustomerItemRegister]")
               .Where("[merchCustomerItemRegister].[consumerKey] = '" + key.ToString() + "' AND [merchCustomerItemRegister].[registerTfKey] = '" + basketTypeKey.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
               .From<CustomerItemRegisterDto>();

            var query = Query<ICustomerItemRegister>.Builder.Where(x => x.ConsumerKey == key && x.CustomerRegisterTfKey == basketTypeKey);
            var translated = TranslateQuery(sql, query);

            //// Assert
            Assert.AreEqual(expected.SQL, translated.SQL);
        }
    }
}
