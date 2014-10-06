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
    public class ProductVariantSqlClausesTests  : BaseUsingSqlServerSyntax<IProductVariant>
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
                .InnerJoin("[merchProductVariantIndex]")
                .On("[merchProductVariant].[pk] = [merchProductVariantIndex].[productVariantKey]")
                .Where("[merchProductVariant].[master]= @0 AND [merchProduct].[pk] = @1", new object[] { 1 , key });

            Console.Write(expected.SQL);

            var sql = new Sql();
            sql.Select("*")
               .From<ProductDto>()
               .InnerJoin<ProductVariantDto>()
               .On<ProductDto, ProductVariantDto>(left => left.Key, right => right.ProductKey)
               .InnerJoin<ProductVariantIndexDto>()
               .On<ProductVariantDto, ProductVariantIndexDto>(left => left.Key, right => right.ProductVariantKey)
               .Where<ProductVariantDto>(x => x.Master)
               .Where<ProductDto>(x => x.Key == key);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}
