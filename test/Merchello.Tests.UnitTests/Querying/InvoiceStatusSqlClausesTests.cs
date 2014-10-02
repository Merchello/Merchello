using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    
    [TestFixture]
    [Category("SqlSyntax")]
    public class InvoiceStatusSqlClausesTests : BaseUsingSqlServerSyntax<IInvoiceStatus>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="InvoiceStatusDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_InvoiceStatus_Base_Sql_Clause()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoiceStatus]")
                .Where("[merchInvoiceStatus].[pk] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceStatusDto>()
                .Where<InvoiceStatusDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}
