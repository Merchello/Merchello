using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.UnitTests.Querying
{
    [TestFixture]
    [Category("SqlSyntax")]
    public class PaymentSqlClausesTests : BaseUsingSqlServerSyntax<IPayment>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="PaymentDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Payment_Base_Sql_Clause()
        {
            //// Arrange
            var id = 10;

            var expected = new Sql();
            expected.Select("*")
                .From("[merchPayment]")
                .InnerJoin("[merchCustomer]").On("[merchPayment].[customerId] = [merchCustomer].[id]")                
                .Where("[merchPayment].[id] = " + id.ToString());

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<PaymentDto>()
                .InnerJoin<CustomerDto>()
                .On<PaymentDto, CustomerDto>(left => left.CustomerId, right => right.Id)                                
                .Where<PaymentDto>(x => x.Id == id);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

    }
}
