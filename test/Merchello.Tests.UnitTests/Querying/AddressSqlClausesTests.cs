using System;
using Lucene.Net.Search;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Querying;
using Merchello.Tests.Base.SqlSyntax;
using Rhino.Mocks;
using Umbraco.Core.Persistence;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Querying
{
    
    [TestFixture]
    [Category("SqlSyntax")]
    public class AddressSqlClausesTests  : BaseUsingSqlServerSyntax<ICustomerAddress>
    {
        /// <summary>
        /// Test to verify that the typed <see cref="CustomerAddressDto"/> query matches generic "select * ..." query 
        /// </summary>
        [Test]
        public void Can_Verify_Address_Base_Clause()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerAddress]")
                .Where("[merchCustomerAddress].[pk] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerAddressDto>()
                .Where<CustomerAddressDto>(x => x.Key == key);

            //// Assert
            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));
        }

        /// <summary>
        /// Test to verify that the typed <see cref="ICustomerAddress"/> query for addresses by customer
        /// </summary>
        [Test]
        public void Can_Verify_Sql_For_Address_By_Customer_Query()
        {
            //// Arrange
            var key = Guid.NewGuid();

            var expected = new Sql();
            expected.Select("*")
                .From("[merchCustomerAddress]")
                .Where("[merchCustomerAddress].[customerKey] = @0", new { key });

            //// Act
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerAddressDto>()
                .Where<CustomerAddressDto>(x => x.CustomerKey == key);

            
            //// Assert
            Assert.AreEqual(expected.SQL, sql.SQL);
        }
    }
}
