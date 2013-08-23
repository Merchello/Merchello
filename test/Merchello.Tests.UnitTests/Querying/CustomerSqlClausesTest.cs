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
    public class CustomerSqlClausesTest : BaseUsingSqlServerSyntax
    {
        [Test]
        public void Can_Verify_Base_Clause()
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
    }
}
