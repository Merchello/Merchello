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
    public class CustomerSqlClausesTest : BaseUsingSqlCeSyntax
    {
        [Test]
        public void Can_Verify_Base_Clause()
        {
            var pk = Guid.Empty;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomer]")
                .Where("[merchCustomer].[pk] = '" + pk.ToString() + "'");

            var sql = new Sql();
            sql.Select("*")
                .From<CustomerDto>()
                .Where<CustomerDto>(x => x.Pk == pk);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}
