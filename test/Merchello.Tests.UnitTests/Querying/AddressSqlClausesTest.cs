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
    public class AddressSqlClausesTest : BaseUsingSqlServerSyntax
    {
        [Test]
        public void Can_Verify_Base_Clause()
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
