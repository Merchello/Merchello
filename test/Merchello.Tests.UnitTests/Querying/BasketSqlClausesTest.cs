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
        [Test]
        public void Can_Verify_Base_Basket_Clause()
        {
            var id = 12;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchBasket]")
                .Where("[merchBasket].[id] = " + id.ToString());

            var sql = new Sql();
            sql.Select("*")
                .From<BasketDto>()
                .Where<BasketDto>(x => x.Id == id);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        [Test]
        public void Can_Verify_Base_BasketItem_Clause()
        {
            var id = 125;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchBasketItem]")
                .Where("[merchBasketItem].[id] = " + id.ToString());

            var sql = new Sql();
            sql.Select("*")
                .From<BasketItemDto>()
                .Where<BasketItemDto>(x => x.Id == id);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}
