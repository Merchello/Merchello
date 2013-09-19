using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class BasketRelatedSqlClausesTest : BaseUsingSqlServerSyntax
    {
        /// <summary>
        /// Test to verify that the typed <see cref="BasketDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Base_Basket_Clause()
        {
            //// Arrange
            var id = 12;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchBasket]")
                .Where("[merchBasket].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<BasketDto>()
                .Where<BasketDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="BasketItemDto"/> query matches generic "select * ..." query  
        /// </summary>
        [Test]
        public void Can_Verify_Base_BasketItem_Clause()
        {
            //// Arrange
            var id = 125;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchBasketItem]")
                .Where("[merchBasketItem].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<BasketItemDto>()
                .Where<BasketItemDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}
