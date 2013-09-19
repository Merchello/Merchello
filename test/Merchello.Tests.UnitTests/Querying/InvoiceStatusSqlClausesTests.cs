using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    
    [TestFixture]
    [Category("SqlSyntax")]
    public class InvoiceStatusSqlClausesTests : BaseUsingSqlServerSyntax
    {
        /// <summary>
        /// Test to verify that the typed <see cref="InvoiceStatusDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_InvoiceStatus_Base_Sql_Clause()
        {
            //// Arrange
            var id = 1;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoiceStatus]")
                .Where("[merchInvoiceStatus].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceStatusDto>()
                .Where<InvoiceStatusDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
    }
}
