using System;
using Lucene.Net.Search;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.Repositories;
using Merchello.Tests.Base.SqlSyntax;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class InvoiceSqlClausesTest : BaseUsingSqlServerSyntax<IInvoice>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="InvoiceDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Invoice_Base_Sql_Clause()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoice]")
                .InnerJoin("[merchCustomer]").On("[merchCustomer].[pk] = [merchInvoice].[customerKey]")
                .InnerJoin("[merchInvoiceStatus]").On("[merchInvoiceStatus].[pk] = [merchInvoice].[invoiceStatusKey]")
                .Where("[merchInvoice].[pk] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceDto>()
                .InnerJoin<CustomerDto>()
                .On<CustomerDto, InvoiceDto>(left => left.Key, right => right.CustomerKey)
                .InnerJoin<InvoiceStatusDto>()
                .On<InvoiceStatusDto, InvoiceDto>(left => left.Key, right => right.InvoiceStatusKey)
                .Where<InvoiceDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify the typed <see cref="InvoiceDto"/> query for invoice by customer
        /// </summary>
        [Test]
        public void Can_Verify_Sql_For_Invoices_By_Customer_Query()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoice]")
                .Where("[merchInvoice].[customerKey] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceDto>()
                .Where<InvoiceDto>(x => x.CustomerKey == key);


            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }

        /// <summary>
        /// Test to verify the typed <see cref="InvoiceDto" /> query for invoice by invoice status
        /// </summary>
        [Test]
        public void Can_Verify_Sql_For_Invoices_By_InvoiceStatus_Query()
        {
            //// Arrange
            var invoiceStatusKey = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoice]")
                .Where("[merchInvoice].[invoiceStatusKey] = @0", new { invoiceStatusKey });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceDto>()
                .Where<InvoiceDto>(x => x.InvoiceStatusKey == invoiceStatusKey);

            //var query = Query<IInvoice>.Builder.Where(x => x.InvoiceStatusId == invoiceStatusId);
            //var translated = TranslateQuery(sql, query);

            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }

    }
}
