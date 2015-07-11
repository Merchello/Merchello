namespace Merchello.Tests.UnitTests.Querying
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Tests.Base.SqlSyntax;

    using NUnit.Framework;

    using Umbraco.Core.Persistence;

    [TestFixture]
    public class ProductSqlClauseTests : BaseUsingSqlServerSyntax<IProduct>
    {
        //[Test]
        //public void Can_Verify_Search_For_Option_SqlClause()
        //{
        //    var expected = new Sql();
        //    expected.Select("*")
        //        .From("[merchProductVariant]");
        //    expected.Append("INNER JOIN [merchProductVariant2ProductAttribute] ON [merchProductVariant].[pk] = [merchProductVariant2ProductAttribute].[productVariantKey]");
        //    expected.Append("INNER JOIN [merchProductOption] ON [merchProductVariant2ProductAttribute].[optionKey] = [merchProductOption].[pk]");

        //    //Console.WriteLine(expected.SQL);
        //    //Console.WriteLine(string.Empty);
        //    var innerSql = new Sql();
        //        innerSql.Select("*")
        //            .From<ProductVariantDto>()
        //            .InnerJoin<ProductVariant2ProductAttributeDto>()
        //            .On<ProductVariantDto, ProductVariant2ProductAttributeDto>(
        //                left => left.Key,
        //                right => right.ProductVariantKey)
        //            .InnerJoin<ProductOptionDto>()
        //            .On<ProductVariant2ProductAttributeDto, ProductOptionDto>(
        //                left => left.OptionKey,
        //                right => right.Key)
        //            .InnerJoin<ProductAttributeDto>()
        //            .On<ProductVariant2ProductAttributeDto, ProductAttributeDto>(
        //                left => left.ProductAttributeKey,
        //                right => right.Key)
        //            ;
        //    var sql = new Sql();
        //    sql.Select("*").From(innerSql.SQL).Where("[merchProductVariant].[master] = @master", new { @master = true });
   
        //    Console.WriteLine(sql.SQL);

        //    Assert.AreEqual(sql.SQL, expected.SQL);
        //}
    }
}