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

        /// <summary>
        /// Test to verify the typed <see cref="InvoiceDto"/> query for invoice by customer
        /// </summary>
        [Test]
        public void Can_Verify_Sql_For_Invoices_By_Customer_Query()
        {
            //// Arrange
            var key = new Guid("E7ADD433-DF59-42AC-B195-BAF0E4F4392A");

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoice]")
                .Where("[merchInvoice].[customerKey] = '" + key.ToString() + "'");

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceDto>();

            var query = Query<IInvoice>.Builder.Where(x => x.CustomerKey == key);
            var translatedSql = TranslateQuery(sql, query);

            //// Assert
            Assert.AreEqual(expected.SQL, translatedSql.SQL);
        }

        /// <summary>
        /// Test to verify the typed <see cref="InvoiceDto" /> query for invoice by invoice status
        /// </summary>
        [Test]
        public void Can_Verify_Sql_For_Invoices_By_InvoiceStatus_Query()
        {
            //// Arrange
            int invoiceStatusId = 1;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoice]")
                .Where("[merchInvoice].[invoiceStatusId] = " + invoiceStatusId.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceDto>();

            var query = Query<IInvoice>.Builder.Where(x => x.InvoiceStatusId == invoiceStatusId);
            var translated = TranslateQuery(sql, query);

            //// Assert
            Assert.AreEqual(expected.SQL, translated.SQL);
        }

    }
}
