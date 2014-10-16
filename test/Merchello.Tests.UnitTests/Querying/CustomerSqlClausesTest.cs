using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Querying;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class CustomerSqlClausesTest : BaseUsingSqlServerSyntax<ICustomer>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="CustomerDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Customer_Base_Clause()
        {
            //// Arrange
            var key  = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomer]")
                .Where("[merchCustomer].[pk] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerDto>()
                .Where<CustomerDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="AnonymousCustomerDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_AnonymousCustomer_Base_Clause()
        {
            //// Arrange
            var key = Guid.Empty;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchAnonymousCustomer]")
                .Where("[merchAnonymousCustomer].[pk] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<AnonymousCustomerDto>()
                .Where<AnonymousCustomerDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify the sql count customer syntax
        /// </summary>
        [Test]
        public void Can_Verify_A_Customer_Count_Query()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("COUNT(*)")
                    .From("[merchCustomer]")
                    .Where("[merchCustomer].[pk] <> @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("COUNT(*)")
                .From<CustomerDto>()
                .Where<CustomerDto>(x => x.Key != key);

            //var query = Query<ICustomer>.Builder.Where(x => x.Key != Guid.Empty);
            //var translated = TranslateQuery(sql, query);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);

        }

    }
}
