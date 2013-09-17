using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    public class PaymentRelatedSqlClausesTests : BaseUsingSqlServerSyntax
    {

        [Test]
        public void Can_Verify_Payment_Base_Sql_Clause()
        {
            var id = 10;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchPayment]")
                .InnerJoin("[merchCustomer]").On("[merchPayment].[customerKey] = [merchCustomer].[pk]")                
                .Where("[merchPayment].[id] = " + id.ToString());

            var sql = new Sql();
            sql.Select("*")
                .From<PaymentDto>()
                .InnerJoin<CustomerDto>()
                .On<PaymentDto, CustomerDto>(left => left.CustomerKey, right => right.Key)                                
                .Where<PaymentDto>(x => x.Id == id);


            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        [Test]
        public void Can_Verify_Transaction_Base_Sql_Clause()
        {
            var id = 10;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchTransaction]")
                .InnerJoin("[merchPayment]").On("[merchTransaction].[paymentId] = [merchPayment].[id]")
                .InnerJoin("[merchInvoice]").On("[merchTransaction].[invoiceId] = [merchInvoice].[id]")
                .Where("[merchPayment].[id] = " + id.ToString());

            var sql = new Sql();
            sql.Select("*")
                .From<TransactionDto>()
                .InnerJoin<PaymentDto>()
                .On<TransactionDto, PaymentDto>(left => left.PaymentId, right => right.Id)
                .InnerJoin<InvoiceDto>()
                .On<TransactionDto, InvoiceDto>(left => left.InvoiceId, right => right.Id)
                .Where<PaymentDto>(x => x.Id == id);

        }

    }
}
