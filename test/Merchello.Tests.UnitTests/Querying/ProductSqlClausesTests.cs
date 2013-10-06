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
    public class ProductSqlClausesTests  : BaseUsingSqlServerSyntax<IProductVariant>
    {

        [Test]
        public void Can_Verify_Product_Base_Sql_Clause()
        {
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchProduct]")
                .InnerJoin("[merchProductVariant]")
                .On("[merchProduct].[pk] = [merchProductVariant].[productKey]")
                .Where("[merchProductVariant].[template]=1")
                .Where("[merchProduct].[pk] = '" + key.ToString() + "'");

            var sql = new Sql();
            sql.Select("*")
               .From<ProductDto>()
               .InnerJoin<ProductVariantDto>()
               .On<ProductDto, ProductVariantDto>(left => left.Key, right => right.ProductKey)
               .Where<ProductVariantDto>(x => x.Template)
               .Where<ProductDto>(x => x.Key == key);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

    }
}
