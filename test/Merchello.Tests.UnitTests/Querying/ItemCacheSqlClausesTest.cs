﻿using System;
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
    public class ItemCacheSqlClausesTest : BaseUsingSqlServerSyntax<IItemCache>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="ItemCacheDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Base_ItemCache_Clause()
        {
            //// Arrange
            var id = 12;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchItemCache]")
                .Where("[merchItemCache].[id] = " + id.ToString());
                //.Where("[merchCustomerItemCache].[itemCacheTfKey] = '" +  + "'")

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<ItemCacheDto>()
                .Where<ItemCacheDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="ItemCacheItemDto"/> query matches generic "select * ..." query  
        /// </summary>
        [Test]
        public void Can_Verify_Base_ItemCacheItem_Clause()
        {
            //// Arrange
            var id = 125;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchItemCacheItem]")
                .Where("[merchItemCacheItem].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<ItemCacheItemDto>()
                .Where<ItemCacheItemDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify the typed <see cref="IItemCache"/> sql by consumer key queries
        /// </summary>
        [Test]
        public void Can_Verify_ItemCache_By_Entity_Query()
        {
            //// Arrange
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
                .From("[merchItemCache]")
                .Where("[merchItemCache].[entityKey] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<ItemCacheDto>()
                .Where<ItemCacheDto>(x => x.EntityKey == key);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }

        /// <summary>
        /// Test to verify type <see cref="IItemCache"/> sql by consumer and basket type field key
        /// </summary>
        [Test]
        public void Can_Verify_ItemCache_By_Entity_And_BasketType_Query()
        {
            //// Arrange
            var basketTypeKey = TypeFieldMock.ItemCacheBasket.TypeKey;
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
               .From("[merchItemCache]")
               .Where("[merchItemCache].[entityKey] = '" + key.ToString() + "' AND [merchItemCache].[itemCacheTfKey] = '" + basketTypeKey.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<ItemCacheDto>()
                .Where<ItemCacheDto>(x => x.EntityKey == key && x.ItemCacheTfKey == basketTypeKey);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }
    }
}
