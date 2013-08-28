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
    public class CustomerRelatedSqlClausesTest : BaseUsingSqlServerSyntax
    {
        [Test]
        public void Can_Verify_Customer_Base_Clause()
        {
            var key = Guid.Empty;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomer]")
                .Where("[merchCustomer].[pk] = '" + key.ToString() + "'");

            var sql = new Sql();
            sql.Select("*")
                .From<CustomerDto>()
                .Where<CustomerDto>(x => x.Key == key);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }


        [Test]
        public void Can_Verify_AnonymousCustomer_Base_Clause()
        {
            var key = Guid.Empty;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchAnonymous]")
                .Where("[merchAnonymous].[pk] = '" + key.ToString() + "'");

            var sql = new Sql();
            sql.Select("*")
                .From<AnonymousDto>()
                .Where<AnonymousDto>(x => x.Key == key);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }


        [Test]
        public void Can_Verify_Address_Base_Clause()
        {
            var id = 111;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchAddress]")
                .Where("[merchAddress].[id] = 111");

            var sql = new Sql();
            sql.Select("*")
                .From<AddressDto>()
                .Where<AddressDto>(x => x.Id == id);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }



    }
}
