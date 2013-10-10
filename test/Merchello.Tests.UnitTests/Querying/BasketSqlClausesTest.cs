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
    public class CustomerItemCacheSqlClausesTest : BaseUsingSqlServerSyntax<ICustomerItemCache>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="CustomerItemCacheDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Base_ItemCache_Clause()
        {
            //// Arrange
            var id = 12;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerItemCache]")
                .Where("[merchCustomerItemCache].[id] = " + id.ToString());
                //.Where("[merchCustomerItemCache].[itemCacheTfKey] = '" +  + "'")

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemCacheDto>()
                .Where<CustomerItemCacheDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="CustomerItemCacheItemDto"/> query matches generic "select * ..." query  
        /// </summary>
        [Test]
        public void Can_Verify_Base_ItemCacheItem_Clause()
        {
            //// Arrange
            var id = 125;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerItemCacheItem]")
                .Where("[merchCustomerItemCacheItem].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemCacheItemDto>()
                .Where<CustomerItemCacheItemDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify the typed <see cref="ICustomerItemCache"/> sql by consumer key queries
        /// </summary>
        [Test]
        public void Can_Verify_ItemCache_By_Consumer_Query()
        {
            //// Arrange
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerItemCache]")
                .Where("[merchCustomerItemCache].[consumerKey] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemCacheDto>()
                .Where<CustomerItemCacheDto>(x => x.ConsumerKey == key);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }

        /// <summary>
        /// Test to verify type <see cref="ICustomerItemCache"/> sql by consumer and basket type field key
        /// </summary>
        [Test]
        public void Can_Verify_ItemCache_By_Consumer_And_BasketType_Query()
        {
            //// Arrange
            var basketTypeKey = TypeFieldMock.ItemCacheBasket.TypeKey;
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
               .From("[merchCustomerItemCache]")
               .Where("[merchCustomerItemCache].[consumerKey] = '" + key.ToString() + "' AND [merchCustomerItemCache].[itemCacheTfKey] = '" + basketTypeKey.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemCacheDto>()
                .Where<CustomerItemCacheDto>(x => x.ConsumerKey == key && x.ItemCacheTfKey == basketTypeKey);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }
    }
}
