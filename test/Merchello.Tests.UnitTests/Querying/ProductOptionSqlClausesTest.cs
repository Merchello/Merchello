namespace Merchello.Tests.UnitTests.Querying
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Tests.Base.SqlSyntax;

    using NUnit.Framework;

    using Umbraco.Core.Persistence;

    [TestFixture]
    public class ProductOptionSqlClausesTest : BaseUsingSqlServerSyntax<IProduct>
    {
        [Test]
        public void Can_Verify_Distinct_ProductAttribute_Query()
        {
            var optionKey = Guid.NewGuid();
            var productKey = Guid.NewGuid();

            var sql = new Sql();
            sql.Select("*")
                .From<ProductAttributeDto>()
                .Append("WHERE [merchProductAttribute].[pk] IN (")
                .Append("SELECT DISTINCT(productAttributeKey)")
                .Append("FROM [merchProductVariant2ProductAttribute]")
                .Append("WHERE optionKey = @okey", new { @okey = optionKey })
                .Append("AND productVariantKey IN (")
                .Append("SELECT [merchProductVariant].[pk] IN (")
                .Append("FROM [merchProductVariant]")
                .Append("WHERE productKey = @pvk", new { @pvk = productKey })
                .Append("AND [master] = @m", new { @m = false })
                .Append(")) ORDER BY sortOrder");

            Console.Write(sql.SQL);
        }
    }
}