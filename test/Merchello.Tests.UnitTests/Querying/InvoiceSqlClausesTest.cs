using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    public class InvoiceSqlClausesTest  : BaseUsingSqlServerSyntax
    {
        /// <summary>
        /// Test to verify that the typed <see cref="InvoiceDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Invoice_Base_Sql_Clause()
        {
            //// Arrange
            var id = 10;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoice]")
                .InnerJoin("[merchCustomer]").On("[merchCustomer].[pk] = [merchInvoice].[customerKey]")
                .InnerJoin("[merchInvoiceStatus]").On("[merchInvoiceStatus].[id] = [merchInvoice].[invoiceStatusId]")
                .Where("[merchInvoice].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceDto>()
                .InnerJoin<CustomerDto>()
                .On<CustomerDto, InvoiceDto>(left => left.Key, right => right.CustomerKey)
                .InnerJoin<InvoiceStatusDto>()
                .On<InvoiceStatusDto, InvoiceDto>(left => left.Id, right => right.InvoiceStatusId)
                .Where<InvoiceDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }
        
    }
}
