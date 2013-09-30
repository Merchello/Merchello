using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class ProductRelatedSqlClausesTests  : BaseUsingSqlServerSyntax<IProductActual>
    {

        [Test]
        public void Can_Verify_Product_Base_Sql_Clause()
        {
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchProductActual]")
                .Where("[merchProductActual].[pk] = '" + key.ToString() + "'");

            var sql = new Sql();
            sql.Select("*")
                .From<ProductActualDto>()
                .Where<ProductActualDto>(x => x.Key == key);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

    }
}
