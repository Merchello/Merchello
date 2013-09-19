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
        /// <summary>
        /// Test to verify that the typed <see cref="CustomerDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Customer_Base_Clause()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomer]")
                .Where("[merchCustomer].[pk] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerDto>()
                .Where<CustomerDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="AnonymousDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_AnonymousCustomer_Base_Clause()
        {
            //// Arrange
            var key = Guid.Empty;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchAnonymous]")
                .Where("[merchAnonymous].[pk] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<AnonymousDto>()
                .Where<AnonymousDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

    }
}
