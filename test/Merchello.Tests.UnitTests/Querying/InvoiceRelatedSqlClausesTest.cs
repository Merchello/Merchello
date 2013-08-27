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
    public class InvoiceRelatedSqlClausesTest  : BaseUsingSqlServerSyntax
    {

        [Test]
        public void Can_Verify_Invoice_Base_Sql_Clause()
        {
            var id = 10;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoice]")
                .InnerJoin("[merchCustomer]").On("[merchCustomer].[pk] = [merchInvoice].[customerKey]")
                .InnerJoin("[merchInvoiceStatus]").On("[merchInvoiceStatus].[id] = [merchInvoice].[invoiceStatusId]")
                .Where("[merchInvoice].[id] = " + id.ToString());

            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceDto>()
                .InnerJoin<CustomerDto>()
                .On<CustomerDto, InvoiceDto>(left => left.Key, right => right.CustomerKey)
                .InnerJoin<InvoiceStatusDto>()
                .On<InvoiceStatusDto, InvoiceDto>(left => left.Id, right => right.InvoiceStatusId)
                .Where<InvoiceDto>(x => x.Id == id);


            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }


        [Test]
        public void Can_Verify_InvoiceStatus_Base_Sql_Clause()
        {
            var id = 1;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchInvoiceStatus]")
                .Where("[merchInvoiceStatus].[id] = " + id.ToString());

            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceStatusDto>()
                .Where<InvoiceStatusDto>(x => x.Id == id);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        
    }
}
